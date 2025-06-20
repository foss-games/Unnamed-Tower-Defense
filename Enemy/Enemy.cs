using System;
using System.Collections;
using System.Data;
using System.Linq;
using Godot;

public partial class Enemy : CharacterBody2D
{
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

    public new bool Visible = false;

    public Vector2 TargetPosition;
    private Label text;

    private float _movementDelta;

    Play play;

    private AStarHexGrid2D AStarHex;

    private Line2D pathLine;

    public override void _Ready()
    {
        text = GetNode<Label>("Label");
        play = (Play)GetTree().GetFirstNodeInGroup("play");
        AStarHex = play.AStarHex;

        pathLine = GetNode<Line2D>("path");

        Speed = 20;
        HP = 20;
        Reward = 5;
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

        //show path, useful for debug
        pathLine.ClearPoints();
        foreach (Vector2 point in path)
        {
            pathLine.AddPoint(pathLine.ToLocal(point));
        }
        pathLine.QueueRedraw();

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
