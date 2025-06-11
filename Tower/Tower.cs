/*
Todo
    



*/

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
    public int Cost;

    public PackedScene projectileScene = GD.Load<PackedScene>("res://Tower/Projectile/Projectile.tscn");
    public Timer ShotTimer;

    private Node2D projectilesNode;

    private TileMapLayer map;
    private TileMapLayer towermask;

    public bool AIEnabled = true;

    public bool IsHeld = false;

    public Sprite2D circle;

    private Play play;

    public override void _Ready()
    {
        RateOfFire = 0.5;
        Damage = 2;
        VisionRange = 5;
        TargetingRange = 3;
        Cost = 5;


        play = (Play)GetTree().GetFirstNodeInGroup("play");

        projectilesNode = (Node2D)GetTree().GetFirstNodeInGroup("projectiles");
        ShotTimer = GetNode<Timer>("ShotTimer");
        circle = GetNode<Sprite2D>("Circle");

        ShotTimer.WaitTime = RateOfFire;

        circle.Scale *= TargetingRange;

        map = (TileMapLayer)GetTree().GetFirstNodeInGroup("background");
        towermask = (TileMapLayer)GetTree().GetFirstNodeInGroup("towermask");
    }

    public void Move(Vector2 destination)
    {
        if (towermask.GetCellTileData(towermask.LocalToMap(destination)) != null)
        {
            return;
        }
        map.SetCell(map.LocalToMap(destination), 0, new Vector2I(4, 0));
        towermask.SetCell(towermask.LocalToMap(destination), 0, new Vector2I(4, 0));

        GlobalPosition = destination;
    }

    public void Destroy()
    {
        map.SetCell(map.LocalToMap(Position), 0, new Vector2I(0, 0));
        QueueFree();
    }

    public void OnShotTimerTimeout()
    {
        if (!AIEnabled) return;
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
        if (!AIEnabled) return;
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