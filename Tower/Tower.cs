using System;
using System.Linq;
using System.Security.Cryptography;
using Godot;
using Godot.Collections;

public partial class Tower : Node2D
{
    public double RateOfFire;
    public int Damage;
    public int TargetingRange;
    public int VisionRange;

    public PackedScene projectileScene = GD.Load<PackedScene>("res://Tower/Projectile/Projectile.tscn");
    public Timer ShotTimer;

    private Node2D projectilesNode;

    private TileMapLayer map;

    public override void _Ready()
    {
        //this is offensive
        projectilesNode = (Node2D)GetTree().GetNodesInGroup("projectiles")[0];
        ShotTimer = GetNode<Timer>("ShotTimer");

        RateOfFire = 0.5;
        Damage = 3;
        VisionRange = 5;
        TargetingRange = 3;

        ShotTimer.WaitTime = RateOfFire;

        map = (TileMapLayer)GetTree().GetFirstNodeInGroup("background");
        map.SetCell(map.LocalToMap(Position), 0, new Vector2I(4, 0));
    }

    public void Destroy()
    {
        map.SetCell(map.LocalToMap(Position), 0, new Vector2I(0, 0));
        QueueFree();
    }

    public void OnShotTimerTimeout()
    {

        var enemies = GetTree().GetNodesInGroup("enemies");
        var targetableEnemies = from Enemy enemy in enemies
                                let d = enemy.GlobalPosition.DistanceSquaredTo(GlobalPosition)
                                where d <= TargetingRange * 1234 * 2
                                orderby d
                                select enemy;

        if (!targetableEnemies.Any()) return;
        //enemy is in range to be shoot
        //shoot
        Enemy target = targetableEnemies.First();

        Projectile p = projectileScene.Instantiate<Projectile>();
        //p.GlobalPosition = GlobalPosition;
        projectilesNode.AddChild(p);
        p.Damage = Damage;
        p.Start(this, target);
    }

    public override void _PhysicsProcess(double delta)
    {
        //look at nearest bad guy in range * 2
        //shoot at bad guy in range until out of range
        //      then pick new bad guy

        //get enemies
        var enemies = GetTree().GetNodesInGroup("enemies");
        if (enemies.Count > 0)
        {
            var visibleEnemies = from Enemy enemy in enemies
                                 let d = enemy.GlobalPosition.DistanceSquaredTo(GlobalPosition)
                                 where d <= VisionRange * 1234 * 2
                                 orderby d
                                 select enemy;

            if (!visibleEnemies.Any()) return;

            Enemy target = visibleEnemies.First();
            LookAt(target.GlobalPosition);
            RotationDegrees += 45;
        }
    }



}
