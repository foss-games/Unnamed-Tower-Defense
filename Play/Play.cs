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
            hud.CurrentWave = value;
            hud.SetTimer(GameDef.Waves[CurrentWave].Interval);
            waveTimer.WaitTime = GameDef.Waves[CurrentWave].Interval;
        }
    }

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
            hud.HP = value;
            if (value == 0)
            {
                Array<Node> enemies = enemiesNode.GetChildren();
                foreach (Enemy enemy in enemies)
                {
                    enemy.QueueFree();
                }
                GD.Print("Game Over");
            }
        }
    }

    private Timer waveTimer;
    private Timer spawnTimer;

    private Node2D towersNode;
    private Node2D enemiesNode;

    private PackedScene enemyScene = GD.Load<PackedScene>("res://Enemy/Enemy.tscn");

    public TileMapLayer map;

    public AStarHexGrid2D AStarHex = new AStarHexGrid2D();
    public List<FOSSGames.Enemy> EnemyTypes = new List<FOSSGames.Enemy>();
    public List<WaveEnemy> SpawnSchedule = [];

    public override void _Ready()
    {
        GD.Print("Play._Ready()");
        //TempInitGD();
        LoadEnemies();
        GameDef = LoadLevel();

        map = GetNode<Node2D>("Background").GetNode<TileMapLayer>("TileMapLayer");
        map.SetCell((Vector2I)GameDef.StartLocation, 0, new Vector2I(2, 0));
        map.SetCell((Vector2I)GameDef.EndLocation, 0, new Vector2I(3, 0));

        InitObstacles();

        hud = GetNode<Hud>("Hud");
        waveTimer = GetNode<Timer>("WaveTimer");
        spawnTimer = GetNode<Timer>("SpawnTimer");

        towersNode = GetNode<Node2D>("Towers");
        enemiesNode = GetNode<Node2D>("Enemies");

        AStarHex.SetupHexGrid(map);

        TileMapLayer towerMask = (TileMapLayer)GetTree().GetFirstNodeInGroup("towermask");
        towerMask.SetCell(towerMask.LocalToMap(GameDef.StartLocation), 0, new Vector2I(2, 0));
        towerMask.SetCell(towerMask.LocalToMap(GameDef.EndLocation), 0, new Vector2I(3, 0));

        waveTimer.WaitTime = GameDef.Waves[0].Interval;
        waveTimer.Start();
        HP = GameDef.MaxHP;
        Credits = GameDef.StartingCredits;

        hud.SetTimer(GameDef.Waves[0].Interval);
        hud.MaxWaves = GameDef.Waves.Count;
        hud.CurrentWave = 0;
        hud.MaxHP = GameDef.MaxHP;
    }

    public void InitObstacles()
    {
        foreach (Vector2 obs in GameDef.Obstacles)
        {
            map.SetCell((Vector2I)obs, 00, new Vector2I(1, 0));
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

    public void LoadEnemies()
    {
        using DirAccess dir = DirAccess.Open("res://Resources/Enemies/");
        if (dir == null) throw new Exception("Unable to load enemies.");

        JsonSerializerOptions options = new JsonSerializerOptions();
        options.Converters.Add(new Vector2Converter());
        options.Converters.Add(new Vector2IConverter());
        options.Converters.Add(new EnemyConverter());

        foreach (string filename in dir.GetFiles())
        {
            if (!filename.EndsWith("json")) continue;
            string json = FileAccess.Open("res://Resources/Enemies/" + filename, FileAccess.ModeFlags.Read).GetAsText();

            EnemyTypes.Add(JsonSerializer.Deserialize<FOSSGames.Enemy>(json, options));
        }
        return;
    }

    public Level LoadLevel()
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.Converters.Add(new Vector2Converter());
        options.Converters.Add(new Vector2IConverter());
        options.Converters.Add(new EnemyConverter());
        using FileAccess file = FileAccess.Open("res://Resources/Levels/1.json", FileAccess.ModeFlags.Read);

        return JsonSerializer.Deserialize<Level>(file.GetAsText(), options);
    }

    public void SpawnEnemy(Guid guid)
    {
        Enemy enemy = enemyScene.Instantiate<Enemy>();
        enemy.GlobalPosition = map.MapToLocal((Vector2I)GameDef.StartLocation);
        enemy.TargetPosition = map.MapToLocal((Vector2I)GameDef.EndLocation);
        enemy.DrawPath = false;
        enemiesNode.AddChild(enemy);
        enemy.LoadStats(guid);
    }
}