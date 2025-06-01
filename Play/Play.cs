using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public partial class Play : Node2D
{
    public GameDef GameDef;

    private CountdownTimer countdownTimer;
    private CurrentWave currentWave;

    private int _currentWave;
    public int CurrentWave
    {
        get
        {
            return _currentWave;
        }
        protected set
        {
            _currentWave = value;
            currentWave.Current++;
        }
    }


    public override void _Ready()
    {

        countdownTimer = (CountdownTimer)GetTree().GetFirstNodeInGroup("countdowntimer");
        currentWave = (CurrentWave)GetTree().GetFirstNodeInGroup("currentwave");

        TempInitGD();

        TileMapLayer map = (TileMapLayer)GetTree().GetNodesInGroup("background")[0];
        map.SetCell(map.LocalToMap(GameDef.StartLocation), 0, new Vector2I(1, 0));
        map.SetCell(map.LocalToMap(GameDef.EndLocation), 0, new Vector2I(2, 0));


        currentWave.SetMax(GameDef.Waves.Length);

    }

    public void NextWave()
    {
        CurrentWave++;
        if (_currentWave == GameDef.Waves.Length - 1)
        {
            //final wave started already
            //stop timer
            countdownTimer.Stop();
            return;
        }
        countdownTimer.Set(GameDef.Waves[_currentWave].Interval);
    }

    public void TempInitGD()
    {
        GameDef gd = new GameDef();

        gd.StartLocation = new Vector2I(68, 957);
        gd.EndLocation = new Vector2I(454, 236);
        //gd.EndLocation

        gd.Waves = [
            new Wave
            {
                Interval = 10,
                Enemies = new List<WaveEnemies>
                {
                    new WaveEnemies{
                        Enemy = new EnemySquare(),
                        Count = 1
                    },
                    new WaveEnemies{
                        Enemy = new EnemyCircle(),
                        Count = 1
                    }
                }
            },
            new Wave
            {
                Interval = 20,
                Enemies = new List<WaveEnemies>
                {
                    new WaveEnemies{
                        Enemy = new EnemySquare(),
                        Count = 1
                    },
                    new WaveEnemies{
                        Enemy = new EnemyCircle(),
                        Count = 1
                    },
                    new WaveEnemies{
                        Enemy = new EnemySquare(),
                        Count = 1
                    },
                }
            },
            new Wave
            {
                Interval = 69,
                Enemies = new List<WaveEnemies>
                {
                    new WaveEnemies{
                        Enemy = new EnemySquare(),
                        Count = 1
                    },
                    new WaveEnemies{
                        Enemy = new EnemyCircle(),
                        Count = 1
                    },
                    new WaveEnemies{
                        Enemy = new EnemySquare(),
                        Count = 1
                    },
                }
            }
        ];
        GameDef = gd;
        GD.Print(JsonSerializer.Serialize(gd));
    }
}

public class GameDef
{
    public Vector2I StartLocation;
    public Vector2I EndLocation;
    public Wave[] Waves;
}

public class Wave
{
    public int Interval;
    public List<WaveEnemies> Enemies;

}

public class WaveEnemies
{
    public Enemy Enemy;
    public int Count;
}

public class EnemySquare : Enemy
{
    public new int HP = 2;
    public new int Speed = 1;
    public new string Name = "Square";
    public new int Reward = 1;
}
public class EnemyCircle : Enemy
{
    public new int HP = 1;
    public new int Speed = 2;
    public new string Name = "Circle";
    public new int Reward = 2;
}

public class Enemy : Entity
{
    public int Reward;
}

public class Entity
{
    public int HP;
    public int Speed;
    public string Name;
    //texture
}