using System;
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

    public NavigationAgent2D NavAgent;

    public Vector2I TargetPosition;
    private Label text;

    private float _movementDelta;

    Play play;

    public override void _Ready()
    {
        NavAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        NavAgent.TargetPosition = TargetPosition;
        text = GetNode<Label>("Label");
        play = (Play)GetTree().GetFirstNodeInGroup("play");


        Speed = 40;
        HP = 20;
        Reward = 5;
    }

    public override void _PhysicsProcess(double delta)
    {
        NavAgent.TargetPosition = TargetPosition;
        // Do not query when the map has never synchronized and is empty.
        if (NavigationServer2D.MapGetIterationId(NavAgent.GetNavigationMap()) == 0)
        {
            return;
        }

        if (NavAgent.IsNavigationFinished())
        {
            return;
        }

        Vector2 nextPathPosition = NavAgent.GetNextPathPosition();
        Vector2 newVelocity = GlobalPosition.DirectionTo(nextPathPosition) * (float)Speed;
        if (NavAgent.AvoidanceEnabled)
        {
            NavAgent.Velocity = newVelocity;
        }
        else
        {
            OnNavCompute(newVelocity);
        }
    }

    public void OnNavCompute(Vector2 velocity)
    {
        Velocity = velocity;
        //GlobalPosition = GlobalPosition.MoveToward(GlobalPosition + velocity, _movementDelta);

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
        //destroy object
        QueueFree();
        play.Credits += Reward;
    }
}
