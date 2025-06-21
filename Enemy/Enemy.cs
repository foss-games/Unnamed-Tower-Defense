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
            //text.Text = value.ToString();
            if (_hp <= 0) Die();
        }
    }
    public double Reward;
    public double Speed;
    public Vector2 TargetPosition;
    private float _movementDelta;
    Play play;
    private AStarHexGrid2D AStarHex;
    private Line2D pathLine;

    public bool DrawPath = false;

    private Sprite2D sprite;

    public override void _Ready()
    {
        play = (Play)GetTree().GetFirstNodeInGroup("play");
        sprite = GetNode<Sprite2D>("sprite");
        AStarHex = play.AStarHex;

        pathLine = GetNode<Line2D>("path");
    }

    public void LoadStats(Guid guid)
    {
        FOSSGames.Enemy stats = play.EnemyTypes.Find(e => e.GUID == guid);

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
        var from = play.map.LocalToMap(GlobalPosition);
        var to = (Vector2I)play.GameDef.EndLocation;
        Vector2[] path = AStarHex.GetPath(from, to);

        if (path.Length < 1)
        {
            return;
        }

        if (DrawPath)
        {
            //show path, useful for debug
            pathLine.ClearPoints();
            foreach (Vector2 point in path)
            {
                pathLine.AddPoint(pathLine.ToLocal(point));
            }
            pathLine.QueueRedraw();
        }

        Vector2 nextPosition = play.map.ToGlobal(path[1]);
        Velocity = Position.DirectionTo(nextPosition) * (float)Speed;

        MoveAndSlide();
    }

    public void OnDestinationReached()
    {
        play.HP--;
        Visible = false; //play animation pls
        QueueFree();
    }

    public void Die()
    {
        Visible = false; //play death animation pls
        play.Credits += Reward;

        GpuParticles2D particles = GD.Load<PackedScene>("res://Enemy/deathparticles.tscn").Instantiate<GpuParticles2D>();
        particles.GlobalPosition = GlobalPosition;
        GetParent().AddChild(particles);

        //destroy object 
        QueueFree();
    }

    public void CreateParticles()
    {

    }

}
