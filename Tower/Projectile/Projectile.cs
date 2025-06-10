using System;
using System.Runtime.CompilerServices;
using Godot;

public partial class Projectile : CharacterBody2D
{
    private Enemy target;
    private double Speed;
    public double Damage;

    public override void _Ready()
    {
        Speed = 5;
    }

    public void Start(Tower origin, Enemy target)
    {
        this.target = target;
        GlobalPosition = origin.GlobalPosition;
        //make visible
        Visible = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsInstanceValid(target))
        {
            //target died while this projectile was in flight
            Visible = false;
            QueueFree();
            return;
        }
        //move to target
        KinematicCollision2D collision = MoveAndCollide((GlobalPosition - target.GlobalPosition) * (float)Speed * (float)delta * -1);

        if (collision != null)
        {
            Enemy collider = (Enemy)collision.GetCollider();
            collider.HP -= Damage;
            Visible = false; //play an animation instead?
            QueueFree();
        }
    }

}
