using System;
using FOSSGames;
using Godot;

public partial class Enemy : CharacterBody2D
{
    public Guid GUID;
    public int Level = 1;
    public EnemyUpgrade Upgrade;
    private double _hp;
    public double HP
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
            EmitSignal(SignalName.HPChanged, value);
            if (_hp <= 0) EmitSignal(SignalName.Killed);
        }
    }
    [Signal]
    public delegate void HPChangedEventHandler(int newHP);
    [Signal]
    public delegate void KilledEventHandler();
    public double Reward;
    public double Speed;
    Play play;
    private AStarHexGrid2D AStarHex;
    private Line2D pathLine;
    private Sprite2D sprite;

    private EnemyStates _state;
    public EnemyStates state
    {
        get { return _state; }
        set
        {
            _state = value;
            EmitSignal(SignalName.StateChanged);
        }
    }
    [Signal]
    public delegate void StateChangedEventHandler();
    [Signal]
    public delegate void ReachedDestinationEventHandler();


    private Vector2 tileMapLayerOffset;

    public override void _Ready()
    {
        play = (Play)GetTree().GetFirstNodeInGroup("play");
        sprite = GetNode<Sprite2D>("sprite");
        AStarHex = play.AStarHex;

        pathLine = GetNode<Line2D>("path");

        ReachedDestination += OnDestinationReached;
        play.LevelLost += OnLevelLost;

        state = EnemyStates.Enabled;

        Killed += OnKilled;
        ReachedDestination += OnDestinationReached;
        tileMapLayerOffset = GetTree().GetFirstNodeInGroup("background").GetNode<TileMapLayer>("TileMapLayer").Position;
    }

    public void LoadStats(Guid guid)
    {
        FOSSGames.Enemy stats = Global.Instance.EnemyTypes.Find(e => e.GUID == guid);

        HP = stats.HP;
        Speed = stats.Speed;
        Upgrade = stats.Upgrade;
        Reward = stats.Reward;

        sprite.Frame = stats.Sprite.Frame;
        sprite.Scale = stats.Sprite.Scale;
        sprite.Modulate = stats.Sprite.Modulate;

        Visible = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (state == EnemyStates.Enabled)
        {
            Vector2I from = play.map.LocalToMap(GlobalPosition);
            Vector2I to = (Vector2I)play.GameDef.EndLocation;

            Vector2 destLocal = play.map.MapToLocal(to);
            float tSize = play.map.TileSet.TileSize.X / 2;

            if (destLocal.X - tSize <= Position.X &&
                destLocal.X + tSize >= Position.X &&
                destLocal.Y - tSize <= Position.Y &&
                destLocal.Y + tSize >= Position.Y)
            {
                EmitSignal(SignalName.ReachedDestination);
                return;
            }

            Vector2[] path = AStarHex.GetPath(from, to);

            if (path.Length < 1)
            {
                return;
            }

            if (Global.Instance.Debug)
            {
                //show path, useful for debug
                pathLine.ClearPoints();
                foreach (Vector2 point in path)
                {
                    pathLine.AddPoint(pathLine.ToLocal(point) + tileMapLayerOffset);
                }
                pathLine.QueueRedraw();
            }

            Vector2 nextPosition = play.map.ToGlobal(path[1]);
            Velocity = Position.DirectionTo(nextPosition) * (float)Speed;

            MoveAndSlide();
            return;
        }
        if (state == EnemyStates.Celebrating)
        {
            Rotation += 2 * (float)delta;
        }
    }

    public void OnDestinationReached()
    {
        Visible = false; //play animation pls
        QueueFree();
    }

    public void OnKilled()
    {
        Visible = false; //play death animation pls
        GpuParticles2D particles = GD.Load<PackedScene>("res://Enemy/deathparticles.tscn").Instantiate<GpuParticles2D>();
        particles.GlobalPosition = GlobalPosition;
        GetParent().AddChild(particles);
        QueueFree();
    }

    public void OnLevelLost()
    {
        state = EnemyStates.Celebrating;
    }
}
