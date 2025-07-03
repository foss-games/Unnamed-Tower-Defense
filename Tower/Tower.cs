using System.Linq;
using FOSSGames;
using Godot;

public partial class Tower : Node2D
{
    public double RateOfFire;
    public double Damage;
    public double TargetingRange;
    public double VisionRange;
    public double Cost;

    public PackedScene projectileScene = GD.Load<PackedScene>("res://Tower/Projectile/Projectile.tscn");
    public Timer ShotTimer;

    private Node2D projectilesNode;

    private AStarHexGrid2D AStarHex;

    public Sprite2D circle;

    private Play play;
    public FOSSGames.Tower TowerType;

    private Vector2I offset = new Vector2I(0, -60);

    public TowerState State = TowerState.Disabled;
    [Signal]
    public delegate void TowerStateChangedEventHandler(TowerState oldState, TowerState newState);

    public override void _Ready()
    {
        play = (Play)GetTree().GetFirstNodeInGroup("play");

        AStarHex = play.AStarHex;

        projectilesNode = (Node2D)GetTree().GetFirstNodeInGroup("projectiles");
        ShotTimer = GetNode<Timer>("ShotTimer");
        circle = GetNode<Sprite2D>("Circle");

        VisionRange = TowerType.VisionRange;
        RateOfFire = TowerType.Attacks[0].ROF;
        Damage = TowerType.Attacks[0].Damage;
        TargetingRange = TowerType.Attacks[0].Range;


        ShotTimer.WaitTime = RateOfFire;

        circle.Scale *= (float)TargetingRange;

        TowerStateChanged += OnStateChange;

        Sprite2D turret = GetNode<Sprite2D>("Turret");
        Sprite2D body = GetNode<Sprite2D>("Body");

        circle.Visible = true;


        // body.Texture = new AtlasTexture
        // {
        //     Atlas = GD.Load<CompressedTexture2D>("res://Resources/towers/TowerTileSet.png"),
        //     Region = new Rect2(0, TowerType.Sprite.Frame * 32, 32, 32)
        // };

        // turret.Texture = new AtlasTexture
        // {
        //     Atlas = GD.Load<CompressedTexture2D>("res://Resources/towers/TowerTileSet.png"),
        //     Region = new Rect2(32, TowerType.Sprite.Frame * 32, 32, 32)
        // };

        // Visible = true;
        //State = TowerState.Enabled;

        QueueRedraw();
    }

    public bool PlacementWillBlockPath(Vector2I destination)
    {
        AStarHexGrid2D astar = new AStarHexGrid2D();
        astar.SetupHexGrid(play.map);

        int cellID = astar.CoordsToID(destination);
        foreach (long connId in astar.GetPointConnections(cellID))
        {
            astar.DisconnectPoints(cellID, connId, false);
        }
        astar.RemoveHexPoint(destination);

        Vector2[] path = astar.GetPath((Vector2I)play.GameDef.StartLocation, (Vector2I)play.GameDef.EndLocation);

        return path.Length < 1;
    }

    private void RemovePointFromNavigation(Vector2I destination)
    {
        int cellID = play.AStarHex.CoordsToID(destination);
        foreach (long connId in play.AStarHex.GetPointConnections(cellID))
        {
            play.AStarHex.DisconnectPoints(cellID, connId, true);
        }
        play.AStarHex.RemovePoint(cellID);
    }

    public void Destroy()
    {
        play.map.SetCell(play.map.LocalToMap(Position), 0, new Vector2I(0, 0));
        //remove placed data from map
        QueueFree();
    }
    public void OnShotTimerTimeout()
    {
        if (State != TowerState.Enabled) return;
        var enemies = GetTree().GetNodesInGroup("enemies");
        var targetableEnemies = from Enemy enemy in enemies
                                let d = enemy.GlobalPosition.DistanceTo(GlobalPosition)
                                where d <= TargetingRange * 32 //FIX ME--------------
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
        switch (State)
        {
            case TowerState.Disabled:
                break;
            case TowerState.Enabled:
                LookAtEnemy();
                break;
            case TowerState.Dragging:
                if (Input.IsActionPressed("Click"))
                    Drag();
                else
                    PlaceTower(GetGlobalMousePosition() + offset);
                break;
            case TowerState.Upgrading:
                break;
        }

    }
    private void OnStateChange(TowerState oldState, TowerState newState)
    {
        if (oldState == TowerState.Dragging)
        {
            //handle dropped
            PlaceTower(GetGlobalMousePosition());
        }
    }
    public void PlaceTower(Vector2 destination) => PlaceTower(play.map.LocalToMap(destination));
    public void PlaceTower(Vector2I destination)
    {
        TileData tileData = play.map.GetCellTileData(destination);

        if (tileData == null ||
            (bool)tileData.GetCustomData("solid") ||
            (bool)tileData.GetCustomData("startpos") ||
            (bool)tileData.GetCustomData("endpos"))
        {
            QueueFree();
        }
        if (PlacementWillBlockPath(destination)) QueueFree();

        circle.Visible = false;

        RemovePointFromNavigation(destination);

        GlobalPosition = play.map.MapToLocal(destination);

        play.map.SetCell(destination, 0, new Vector2I(1, 0));

        play.Credits -= Cost;

        State = TowerState.Enabled;
    }
    private void LookAtEnemy()
    {
        var enemies = GetTree().GetNodesInGroup("enemies");
        if (enemies.Count > 0)
        {
            var visibleEnemies = from Enemy enemy in enemies
                                 let d = enemy.GlobalPosition.DistanceTo(GlobalPosition)
                                 where d <= VisionRange * 32
                                 orderby d
                                 select enemy;

            if (!visibleEnemies.Any()) return;

            Enemy target = visibleEnemies.First();
            LookAt(target.GlobalPosition);
        }
    }
    private void Drag()
    {
        GlobalPosition = GetGlobalMousePosition() + offset;
        QueueRedraw();
    }
}

public enum TowerState
{
    Enabled,
    Disabled,
    Dragging,
    Upgrading
}