using System.Collections.Generic;
using System.Linq;
using System.Xml.Resolvers;
using FOSSGames;
using Godot;
using Godot.Collections;

public partial class Tower : Node2D
{
    public double RateOfFire;
    public double Damage;
    public double TargetingRange;
    public double VisionRange;
    public double Cost;
    public int Level = 1;
    [Signal]
    public delegate void LevelChangedEventHandler();
    public PackedScene projectileScene = GD.Load<PackedScene>("res://Tower/Projectile/Projectile.tscn");
    public Timer ShotTimer;
    private Node2D projectilesNode;
    private AStarHexGrid2D AStarHex;
    private Sprite2D body;
    private Sprite2D turret;
    public int TurretFrame
    {
        get
        {
            return turret.Frame;
        }
    }
    public Sprite2D Circle;
    private Line2D beam;
    private Play play;
    public FOSSGames.Tower TowerType;
    private Vector2I offset = new Vector2I(0, -80);
    public TowerState State = TowerState.Disabled;
    [Signal]
    public delegate void StateChangedEventHandler(TowerState oldState, TowerState newState);
    private Vector2 tileMapLayerOffset;

    //AudioStreamMP3 sound = AudioStreamMP3.New();
    public override void _Ready()
    {
        play = (Play)GetTree().GetFirstNodeInGroup("play");

        AStarHex = play.AStarHex;

        projectilesNode = (Node2D)play.GetNode<Node2D>("Projectiles");
        ShotTimer = GetNode<Timer>("ShotTimer");
        Circle = GetNode<Sprite2D>("Circle");
        body = GetNode<Sprite2D>("Body");
        turret = GetNode<Sprite2D>("Turret");
        beam = turret.GetNode<Line2D>("Beam");
        GetNode<Button>("Button").Pressed += Clicked;

        VisionRange = TowerType.VisionRange;
        RateOfFire = TowerType.Attacks[0].ROF;
        Damage = TowerType.Attacks[0].Damage;
        TargetingRange = TowerType.Attacks[0].Range;
        Cost = TowerType.Cost;

        tileMapLayerOffset = GetTree().GetFirstNodeInGroup("background").GetNode<TileMapLayer>("TileMapLayer").Position;

        ShotTimer.WaitTime = RateOfFire;

        Circle.Scale *= (float)TargetingRange;

        StateChanged += OnStateChange;

        Circle.Visible = true;

        Modulate = TowerType.Sprite.Modulate;

        body.Texture = (AtlasTexture)body.Texture.Duplicate();
        turret.Texture = (AtlasTexture)turret.Texture.Duplicate();
        (body.Texture as AtlasTexture).Region = new Rect2(0, TowerType.Sprite.Frame * 32, 0, 0);
        (turret.Texture as AtlasTexture).Region = new Rect2(0, TowerType.Sprite.Frame * 32, 0, 0);
    }
    public void Clicked()
    {
        ((UpgradeArea)play.GetNode<Node2D>("UpgradeArea")).Show(this);
        Circle.Visible = true;
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
        astar.RemoveHexPoint(cellID);

        Vector2[] fullPath = astar.GetPath((Vector2I)play.GameDef.StartLocation, (Vector2I)play.GameDef.EndLocation);

        return fullPath.Length < 1;
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
    public void OnShotTimerTimeout()
    {
        if (State != TowerState.Enabled) return;

        var targetableEnemies = GetEnemies();

        if (!targetableEnemies.Any()) return;
        Enemy target = targetableEnemies.First();

        switch (TowerType.Attacks[0].Type)
        {
            case FOSSGames.AttackTypes.Projectile:
                DoProjectileShot(target);
                break;
            case FOSSGames.AttackTypes.Beam:
                DoBeamShot(target);
                break;
            case FOSSGames.AttackTypes.Missile:
                break;
            case FOSSGames.AttackTypes.AOE:
                DoAOEShot();
                break;
        }
    }
    private IEnumerable<Enemy> GetEnemies()
    {
        var enemies = GetTree().GetNodesInGroup("enemies");
        return from Enemy enemy in enemies
               let d = enemy.GlobalPosition.DistanceTo(GlobalPosition)
               where d <= TargetingRange * 32 //FIX ME--------------
               orderby d
               select enemy;
    }
    private void DoProjectileShot(Enemy target)
    {
        Projectile p = projectileScene.Instantiate<Projectile>();
        Sprite2D projSprite = p.GetNode<Sprite2D>("Sprite2D");
        projSprite.Frame = (TowerType.Sprite.Frame + 1) * 7;
        projSprite.Modulate = TowerType.Sprite.Modulate;
        projectilesNode.AddChild(p);
        p.Damage = Damage;

        //var file = FileAccess.Open("res://Resources/Towers/basic.mp3", FileAccess.ModeFlags.Read);
        //var sound = 
        AudioStreamPlayer2D play = new AudioStreamPlayer2D()
        {
            Stream = AudioStreamMP3.LoadFromFile("res://Resources/Towers/basic.mp3"),
            VolumeLinear = 0.5f
        };
        AddChild(play);
        play.Play();
        //sound.Loop = false;


        p.Start(this, target);
    }
    private void DoAOEShot()
    {
        IEnumerable<Enemy> enemies = GetEnemies();
        if (!enemies.Any()) return;
        foreach (Enemy enemy in GetEnemies()) enemy.HP -= Damage;
        Sprite2D wave = (Sprite2D)body.Duplicate();
        AddChild(wave);

        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(wave, "modulate", new Color(Modulate.R, Modulate.G, Modulate.B, 0.5f), 0.75f);
        tween.Parallel().TweenProperty(wave, "scale", new Vector2((float)(TargetingRange * 1.5), (float)(TargetingRange * 1.5)), 0.75f);
        tween.TweenCallback(Callable.From(wave.QueueFree));
    }
    private void DoBeamShot(Enemy target)
    {

        //todo convert to shapecast2d
        beam.Modulate = Modulate;
        beam.Visible = true;

        PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
        Array<Rid> hitEnemies = [.. GetTree().GetNodesInGroup("projectiles").Select(p => ((Projectile)p).GetRid())];

        for (int i = 0; i < GetTree().GetNodeCountInGroup("enemies"); i++)
        {
            var query = PhysicsRayQueryParameters2D.Create(GlobalPosition, target.GlobalPosition);
            query.Exclude = hitEnemies;
            var result = spaceState.IntersectRay(query);
            if (result.Count < 1) continue;

            Enemy hit = (Enemy)result["collider"];

            hitEnemies.Add(hit.GetRid());
            AudioStreamPlayer2D play = new AudioStreamPlayer2D()
            {
                Stream = AudioStreamMP3.LoadFromFile("res://Resources/Towers/beam.mp3"),
                VolumeLinear = 0.25f
            };
            AddChild(play);
            play.Play();
            hit.HP -= Damage;
        }

        Timer timer = new()
        {
            WaitTime = 0.15,
            Autostart = false,
            OneShot = true,
        };
        timer.Timeout += () => { beam.Visible = false; };
        AddChild(timer);
        timer.Start();
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
            return;
        }
        if (PlacementWillBlockPath(destination) ||
            play.Credits < Cost)
        {
            QueueFree();
            return;
        }

        Circle.Visible = false;

        RemovePointFromNavigation(destination);
        play.map.SetCell(destination, 0, new Vector2I(1, 0));

        GlobalPosition = play.map.MapToLocal(destination) + tileMapLayerOffset;

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
            turret.LookAt(target.GlobalPosition);
        }
    }
    public void Upgrade()
    {
        if (Level > TowerType.Upgrades.Length)
            return;

        Upgrade upgrade = TowerType.Upgrades[Level - 1];

        if (play.Credits < upgrade.Cost)
            return;

        foreach (UpgradeEffect effect in upgrade.UpgradeEffects)
        {
            switch (effect.Effect)
            {
                case UpgradeEffectEffects.Damage:
                    Damage += effect.Value;
                    break;
                case UpgradeEffectEffects.DamageMult:
                    Damage *= effect.Value;
                    break;
                case UpgradeEffectEffects.RateOfFire:
                    RateOfFire += effect.Value;
                    ShotTimer.WaitTime = RateOfFire;
                    break;
                case UpgradeEffectEffects.RateOfFireMult:
                    RateOfFire *= effect.Value;
                    ShotTimer.WaitTime = RateOfFire;
                    break;
                case UpgradeEffectEffects.TargetingRange:
                    TargetingRange += effect.Value * 32;
                    //update targeting circle
                    Circle.Scale = new Vector2((float)TargetingRange, (float)TargetingRange);
                    break;
                case UpgradeEffectEffects.TargetingRangeMult:
                    TargetingRange *= effect.Value;
                    //update targeting circle
                    Circle.Scale = new Vector2((float)TargetingRange, (float)TargetingRange);
                    break;
            }
        }
        turret.Frame++;
        Level++;
        play.Credits -= upgrade.Cost;
        EmitSignal(SignalName.LevelChanged);
    }
    private void Drag()
    {
        GlobalPosition = GetGlobalMousePosition() + offset;
        QueueRedraw();
    }
    public void Destroy()
    {
        play.map.SetCell(play.map.LocalToMap(Position), 0, new Vector2I(0, 0));
        QueueFree();
    }
}

public enum TowerState
{
    Enabled,
    Disabled,
    Dragging,
    Upgrading
}