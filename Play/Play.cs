using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using FOSSGames;
using Godot;
using Godot.Collections;

public partial class Play : Node2D
{
    public Level GameDef;
    private int _currentWave = -1;
    public int CurrentWave
    {
        get
        {
            return _currentWave;
        }
        set
        {
            _currentWave = value;
            EmitSignal(SignalName.WaveChanged, _currentWave);
        }
    }

    [Signal]
    public delegate void WaveChangedEventHandler(int waveID);

    public double _credits;
    public double Credits
    {
        get
        {
            return _credits;
        }
        set
        {
            _credits = value;
            hud.SetCredits(value);
        }
    }

    public Hud hud;

    private int _hp;
    public int HP
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
            EmitSignal(SignalName.HPChanged, value);
            OnHPChanged(value);
        }
    }

    [Signal]
    public delegate void HPChangedEventHandler();
    [Signal]
    public delegate void LevelLostEventHandler();
    [Signal]
    public delegate void LevelWonEventHandler();
    [Signal]
    public delegate void LevelEndedEventHandler();

    private Timer waveTimer;
    private Timer spawnTimer;

    private Node2D towersNode;
    private Node2D enemiesNode;

    private PackedScene enemyScene = GD.Load<PackedScene>("res://Enemy/Enemy.tscn");

    public TileMapLayer map;

    public AStarHexGrid2D AStarHex = new AStarHexGrid2D();
    public List<WaveEnemy> SpawnSchedule = [];

    public LevelState State = LevelState.Running;
    [Signal]
    public delegate void LevelStateChangedEventHandler();

    public override void _Ready()
    {
        GD.Print("Play._Ready()");
        GameDef = Global.Instance.Levels[Global.Instance.SelectedLevelIndex];

        map = GetNode<Node2D>("Background").GetNode<TileMapLayer>("TileMapLayer");
        map.SetCell((Vector2I)GameDef.StartLocation, 0, new Vector2I(3, 0));
        map.SetCell((Vector2I)GameDef.EndLocation, 0, new Vector2I(4, 0));

        InitObstacles();

        hud = GetNode<Hud>("Hud");
        waveTimer = GetNode<Timer>("WaveTimer");
        spawnTimer = GetNode<Timer>("SpawnTimer");

        towersNode = GetNode<Node2D>("Towers");
        enemiesNode = GetNode<Node2D>("Enemies");

        AStarHex.SetupHexGrid(map);

        waveTimer.WaitTime = GameDef.Waves[0].Interval;
        waveTimer.Start();
        HP = GameDef.MaxHP;
        Credits = GameDef.StartingCredits;

        hud.SetTimer(GameDef.Waves[0].Interval);
        hud.MaxWaves = GameDef.Waves.Count;
        hud.CurrentWave = 0;
        hud.MaxHP = GameDef.MaxHP;

        LevelLost += OnLevelLost;
        LevelWon += OnLevelWon;
        LevelEnded += OnLevelEnd;
        WaveChanged += _ =>
        {
            waveTimer.WaitTime = GameDef.Waves[CurrentWave].Interval;
        };
    }

    public override void _Draw()
    {
        foreach (Vector2I cell in map.GetUsedCells())
        {
            //DrawCircle(cell, 3, new Color(255, 0, 0, 255), true);

        }
    }

    public override void _Process(double delta)
    {
        if (State == LevelState.Running &&
            SpawnSchedule.Count < 1 && //no pending spawns
            GameDef.Waves.Count < 1 && //no pending waves
            GetTree().GetNodesInGroup("enemies").Count < 1) //no spawned enemies
        {
            State = LevelState.Complete;
            EmitSignal(SignalName.LevelWon);
        }
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsAction("Click") && GetNode<Node2D>("UpgradeArea").Visible)
        {
            GetNode<Node2D>("UpgradeArea").Visible = false;
        }
    }
    public void InitObstacles()
    {
        foreach (Vector2 obs in GameDef.Obstacles)
        {
            map.SetCell((Vector2I)obs, 0, new Vector2I(2, 0));
        }
    }

    public void NextWave()
    {
        GD.Print("Play.NextWave()");
        waveTimer.Stop();

        if (GameDef.Waves.Count <= 0)
        {
            //no more waves to spawn
            return;
        }

        Wave wave = GameDef.Waves.First();
        GameDef.Waves.Remove(wave);

        //update ui for new wave
        hud.SetTimer(wave.Interval);
        hud.CurrentWave++;

        //update wave timer
        waveTimer.WaitTime = wave.Interval;
        waveTimer.Start();

        lock (SpawnSchedule)
        {
            SpawnSchedule.AddRange(CreateSpawnSchedule(wave));
            SpawnSchedule.Sort((a, b) => a.Interval > b.Interval ? 1 : -1);
        }
    }

    public void OnSpawnTimerTick()
    {
        List<WaveEnemy> completed = [];
        foreach (WaveEnemy spawn in SpawnSchedule)
        {
            if (spawn.Interval <= spawnTimer.WaitTime)
            {
                SpawnEnemy(spawn.GUID);
                if (--spawn.Count <= 0)
                {
                    completed.Add(spawn);
                }
            }
            else
            {
                spawn.Interval -= spawnTimer.WaitTime;
            }
        }
        lock (SpawnSchedule)
        {
            SpawnSchedule.RemoveAll(a => completed.IndexOf(a) > -1);
        }
    }

    public static List<WaveEnemy> CreateSpawnSchedule(Wave wave)
    {
        List<WaveEnemy> schedule = [];
        double delay = 0;
        foreach (WaveEnemy spawn in wave.Enemies)
        {
            for (int i = 0; i < spawn.Count; i++)
            {
                WaveEnemy nspawn = new WaveEnemy()
                {
                    GUID = spawn.GUID,
                    Interval = spawn.Interval + delay
                };
                delay += spawn.Interval;
                schedule.Add(nspawn);
            }
        }
        return schedule;
    }

    public void SpawnEnemy(Guid guid)
    {
        Enemy enemy = enemyScene.Instantiate<Enemy>();
        enemy.GlobalPosition = map.MapToLocal((Vector2I)GameDef.StartLocation);
        enemy.ReachedDestination += () => HP--;
        enemy.Killed += () => Credits += enemy.Reward;
        enemiesNode.AddChild(enemy);
        enemy.LoadStats(guid);
    }

    private void OnHPChanged(int value)
    {
        hud.HP = value;
        if (value <= 0)
        {
            EmitSignal(SignalName.LevelLost);
        }
    }
    private void OnLevelEnd()
    {
        SpawnSchedule.Clear();
        waveTimer.Stop();
        spawnTimer.Stop();
    }
    private void OnLevelLost()
    {
        EmitSignal(SignalName.LevelEnded);
        GetNode<Node2D>("EndMenu").Visible = true;
    }
    private void OnLevelWon()
    {
        EmitSignal(SignalName.LevelEnded);
        GetNode<Node2D>("EndMenu").Visible = true;
        Global.Instance.CompletedLevels.Add(GameDef.GUID.ToString());
        Global.SaveGame();
    }
}