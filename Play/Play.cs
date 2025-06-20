using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FOSSGames;
using Godot;
using Godot.Collections;

public partial class Play : Node2D
{
    public GameDef GameDef;
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

    private TileMapLayer map;

    public PoppableList<Wave> SpawningWaves = new PoppableList<Wave>();

    public override void _Ready()
    {
        GD.Print("Play._Ready()");
        TempInitGD();

        hud = GetNode<Hud>("Hud");
        waveTimer = GetNode<Timer>("WaveTimer");
        spawnTimer = GetNode<Timer>("SpawnTimer");

        towersNode = GetNode<Node2D>("Towers");
        enemiesNode = GetNode<Node2D>("Enemies");

        map = GetNode<Node2D>("Background").GetNode<TileMapLayer>("TileMapLayer");
        map.SetCell(map.LocalToMap(GameDef.StartLocation), 0, new Vector2I(1, 0));
        map.SetCell(map.LocalToMap(GameDef.EndLocation), 0, new Vector2I(2, 0));
        TileMapLayer towerMask = (TileMapLayer)GetTree().GetFirstNodeInGroup("towermask");
        towerMask.SetCell(towerMask.LocalToMap(GameDef.StartLocation), 0, new Vector2I(1, 0));
        towerMask.SetCell(towerMask.LocalToMap(GameDef.EndLocation), 0, new Vector2I(2, 0));



        waveTimer.WaitTime = GameDef.Waves[0].Interval;
        waveTimer.Start();

        hud.SetTimer(GameDef.Waves[0].Interval);
        hud.MaxWaves = GameDef.Waves.Count;
        hud.CurrentWave = 0;
        hud.MaxHP = GameDef.MaxHP;
        HP = GameDef.MaxHP;

        Credits = GameDef.StartingCredits;

        //load json
        FOSSGames.Tower towerDef = JsonSerializer.Deserialize<FOSSGames.Tower>(File.ReadAllText(@"C:\temp\basictower.json"));
        //construct tower
        GD.Print(towerDef);
    }

    public void NextWave()
    {
        GD.Print("Play.NextWave()");
        waveTimer.Stop();

        Wave wave = GameDef.Waves.PopFirst();

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

    public void TempInitGD()
    {
        GameDef gd = new GameDef();

        gd.StartLocation = new Vector2I(68, 757);
        gd.EndLocation = new Vector2I(454, 236);
        gd.StartingCredits = 10;
        gd.MaxHP = 10;
        //gd.EndLocation

        gd.Waves = [
            new Wave
            {
                Interval = 5,
                Enemies = new List<WaveEnemies>
                {
                    new WaveEnemies{
                        Enemy = new Enemy(),
                        Count = 1,
                        Interval = 1.0f
                    },
                    new WaveEnemies{
                        Enemy = new Enemy(),
                        Count = 1,
                        Interval = 1.0f
                    }
                }
            },
            new Wave
            {
                Interval = 5,
                Enemies = new List<WaveEnemies>
                {
                    new WaveEnemies{
                        Enemy = new Enemy(),
                        Count = 1,
                        Interval = 1.0f
                    },
                    new WaveEnemies{
                        Enemy = new Enemy(),
                        Count = 1,
                        Interval = 1.0f
                    },
                    new WaveEnemies{
                        Enemy = new Enemy(),
                        Count = 1,
                        Interval = 1.0f
                    },
                }
            },
            new Wave
            {
                Interval = 10,
                Enemies = new List<WaveEnemies>
                {
                    new WaveEnemies{
                        Enemy = new Enemy(),
                        Count = 1,
                        Interval = 1.0f
                    },
                    new WaveEnemies{
                        Enemy = new Enemy(),
                        Count = 1,
                        Interval = 1.0f
                    },
                    new WaveEnemies{
                        Enemy = new Enemy(),
                        Count = 1,
                        Interval = 1.0f
                    },
                }
            }
        ];
        GameDef = gd;
        //GD.Print(JsonSerializer.Serialize(gd));
    }

    public void OnSpawnTimerTick()
    {
        foreach (Wave wave in SpawningWaves)
        {
            if (wave.SpawnedCount >= wave.Enemies.Count) continue;

            if (wave.TimeSinceLastSpawn >= wave.Enemies[wave.SpawnedCount].Interval)
            {
                SpawnEnemy();
                wave.TimeSinceLastSpawn = 0;
                wave.SpawnedCount++;
                if (wave.SpawnedCount > wave.Enemies.Count)
                {
                    SpawningWaves.Remove(wave);
                }
            }
            wave.TimeSinceLastSpawn += 0.25f;
        }
    }

    public void SpawnEnemy()
    {

        Enemy enemy = enemyScene.Instantiate<Enemy>();
        enemy.GlobalPosition = new Vector2I(GameDef.StartLocation.X - 4, GameDef.StartLocation.Y - 12);
        enemy.Visible = true;
        enemy.TargetPosition = GameDef.EndLocation;

        enemiesNode.AddChild(enemy);
    }
}

public class PoppableList<T> : List<T>
{
    public T PopFirst()
    {
        T item = this[0];
        this.Remove(item);
        return item;
    }
    public T Pop()
    {
        T item = this[this.Count - 1];
        this.Remove(item);
        return item;
    }

    public T PopLast() => Pop();
}

public class GameDef
{
    public Vector2I StartLocation;
    public Vector2I EndLocation;
    public double StartingCredits;
    public int MaxHP;
    public PoppableList<Wave> Waves;
}

public class Wave
{
    public int Interval;
    public float TimeSinceLastSpawn = 0;
    public int SpawnedCount = 0;
    public List<WaveEnemies> Enemies;

}

public class WaveEnemies
{
    public Enemy Enemy;
    public int Count;
    public float Interval = 1.0f;
}

// public class EnemySquare : Enemy
// {
//     public new int HP = 2;
//     public new int Speed = 1;
//     public new string Name = "Square";
//     public new int Reward = 1;
// }
// public class EnemyOctogon : Enemy
// {
//     public new int HP = 5;
//     public new int Speed = 5;
//     public new string Name = "Octogon";
//     public new int Reward = 10;
// }
// public class EnemyCircle : Enemy
// {
//     public new int HP = 1;
//     public new int Speed = 2;
//     public new string Name = "Circle";
//     public new int Reward = 2;
// }

// public class Enemy : Entity
// {
//     public int Reward;
// }

public class Entity
{
    public int HP;
    public int Speed;
    public string Name;
    //texture
}