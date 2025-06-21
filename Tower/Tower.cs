using System.Linq;
using Godot;

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
    private AStarHexGrid2D AStarHex;
    public bool AIEnabled = true;

    public bool IsHeld = false;

    public Sprite2D circle;

    private Play play;

    private NavigationAgent2D NavAgent;

    public override void _Ready()
    {
        RateOfFire = 0.5;
        Damage = 2;
        VisionRange = 5;
        TargetingRange = 3;
        Cost = 5;


        play = (Play)GetTree().GetFirstNodeInGroup("play");

        AStarHex = play.AStarHex;

        projectilesNode = (Node2D)GetTree().GetFirstNodeInGroup("projectiles");
        ShotTimer = GetNode<Godot.Timer>("ShotTimer");
        circle = GetNode<Sprite2D>("Circle");

        ShotTimer.WaitTime = RateOfFire;

        circle.Scale *= TargetingRange;

        map = GetTree().GetFirstNodeInGroup("background").GetNode<TileMapLayer>("TileMapLayer");
        towermask = (TileMapLayer)GetTree().GetFirstNodeInGroup("towermask");
    }

    public bool PlacementWillBlockPath(Vector2I destination)
    {
        using AStarHexGrid2D astar = new AStarHexGrid2D();
        astar.SetupHexGrid(play.map);

        int cellID = astar.CoordsToID(play.map.LocalToMap(destination));
        foreach (long connId in astar.GetPointConnections(cellID))
        {
            astar.DisconnectPoints(cellID, connId, true);
        }
        astar.RemovePoint(cellID);

        Vector2[] path = astar.GetPath((Vector2I)play.GameDef.StartLocation, (Vector2I)play.GameDef.EndLocation);

        return path.Length < 1;
    }

    private void RemovePointFromNavigation(Vector2I destination)
    {
        int cellID = play.AStarHex.CoordsToID(play.map.LocalToMap(destination));
        foreach (long connId in play.AStarHex.GetPointConnections(cellID))
        {
            play.AStarHex.DisconnectPoints(cellID, connId, true);
        }
        play.AStarHex.RemovePoint(cellID);
    }

    public void Move(Vector2I destination)
    {
        if (destination.X == 0 || destination.Y == 0) return;
        if (towermask.GetCellTileData(towermask.LocalToMap(destination)) != null)
        {
            return;
        }
        if (PlacementWillBlockPath(destination)) return;

        map.SetCell(map.LocalToMap(destination), 0, new Vector2I(4, 0));
        towermask.SetCell(towermask.LocalToMap(destination), 0, new Vector2I(4, 0));
        RemovePointFromNavigation(destination);

        GlobalPosition = destination;

        ((Play)GetTree().GetFirstNodeInGroup("play")).Credits -= Cost;
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
                                where d <= TargetingRange * 1234 * 2 //FIX ME--------------
                                orderby d
                                select enemy;

        if (!targetableEnemies.Any()) return;
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

        var enemies = GetTree().GetNodesInGroup("enemies");
        if (enemies.Count > 0)
        {
            var visibleEnemies = from Enemy enemy in enemies
                                 let d = enemy.GlobalPosition.DistanceSquaredTo(GlobalPosition)
                                 where d <= VisionRange * 1234 * 2 //FIX ME--------------
                                 orderby d
                                 select enemy;

            if (!visibleEnemies.Any()) return;

            Enemy target = visibleEnemies.First();
            LookAt(target.GlobalPosition);
            RotationDegrees += 90;
        }
    }
}