using System;
using System.Collections.Generic;
using System.IO;
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

    public List<Wave> SpawningWaves = new List<Wave>();
    public AStarHexGrid2D AStarHex = new AStarHexGrid2D();
    public List<FOSSGames.Enemy> EnemyTypes = new List<FOSSGames.Enemy>();

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

        Wave wave = GameDef.Waves.First();
        GameDef.Waves.Remove(wave);

        SpawningWaves.Add(wave);

        if (GameDef.Waves.Count <= 0)
        {
            //no more waves to spawn
            return;
        }
        //update ui for new wave
        hud.SetTimer(wave.Interval);
        hud.CurrentWave++;

        //update wave timer
        waveTimer.WaitTime = wave.Interval;
        waveTimer.Start();
    }

    public void LoadEnemies()
    {
        using DirAccess dir = DirAccess.Open("res://Enemies/");
        if (dir == null) throw new System.Exception("Unable to load enemies.");

        JsonSerializerOptions options = new JsonSerializerOptions();
        options.Converters.Add(new Vector2Converter());
        options.Converters.Add(new Vector2IConverter());
        options.Converters.Add(new EnemyConverter());

        foreach (string filename in dir.GetFiles())
        {
            string json = Godot.FileAccess.Open("res://Enemies/" + filename, Godot.FileAccess.ModeFlags.Read).GetAsText();

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
        using Godot.FileAccess file = Godot.FileAccess.Open("res://Levels/1.json", Godot.FileAccess.ModeFlags.Read);

        Level level = JsonSerializer.Deserialize<Level>(file.GetAsText(), options);
        return level;
    }

    public void OnSpawnTimerTick()
    {
        foreach (Wave wave in SpawningWaves.ToArray())
        {
            if (wave.SpawnedCount >= wave.Enemies.Count) continue;

            if (wave.TimeSinceLastSpawn >= wave.Enemies[wave.SpawnedCount].Interval)
            {

                SpawnEnemy(wave.Enemies[wave.SpawnedCount].GUID);
                wave.TimeSinceLastSpawn = 0;
                wave.SpawnedCount++;
                if (wave.SpawnedCount >= wave.Enemies.Count)
                {
                    SpawningWaves.Remove(wave);
                }
            }
            wave.TimeSinceLastSpawn += 0.25f;
        }
    }

    public void SpawnEnemy(Guid guid)
    {
        Enemy enemy = enemyScene.Instantiate<Enemy>();
        enemy.GlobalPosition = map.MapToLocal((Vector2I)GameDef.StartLocation);
        enemy.TargetPosition = map.MapToLocal((Vector2I)GameDef.EndLocation);
        enemiesNode.AddChild(enemy);
        enemy.LoadStats(guid);
        //enemy.CallDeferred("LoadStats", guid.ToString());
    }
}