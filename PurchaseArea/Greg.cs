using System;
using Godot;

public partial class Greg : Node2D
{
    public int Speed = 50;
    public bool HasMouse = false;
    public bool IsHeld = false;
    public Button button;
    public Vector2 offset;

    public Vector2 StartingPosition;
    PackedScene towerScene = GD.Load<PackedScene>("res://Tower/Tower.tscn");
    private TileMapLayer map;
    private Node2D towerCollection;
    private Tower tower;
    public override void _Ready()
    {
        StartingPosition = GlobalPosition;
        map = (TileMapLayer)GetTree().GetFirstNodeInGroup("background");
        towerCollection = (Node2D)GetTree().GetFirstNodeInGroup("towerscollection");
        tower = GetNode<Tower>("Tower");
        tower.AIEnabled = false;
        //GetNode<Tower>("Greg").AIEnabled = false;
        button = GetNode<Button>("Button");
        button.ButtonDown += TowerPickedUp;
        button.ButtonUp += TowerDropped;
    }

    public void TowerPickedUp()
    {
        if (!IsHeld)
        {
            GD.Print("Grabbed");
            IsHeld = true;
            offset = GetGlobalMousePosition() - GlobalPosition;
            tower.GetNode<Polygon2D>("Polygon2D").Visible = true;
        }
    }

    public void TowerDropped()
    {
        GD.Print("Dropped");
        GD.Print(map.MapToLocal(map.LocalToMap(GetGlobalMousePosition())));
        IsHeld = false;

        Tower newTower = (Tower)towerScene.Instantiate();
        newTower.CallDeferred("Move", map.MapToLocal(map.LocalToMap(GetGlobalMousePosition())));
        towerCollection.AddChild(newTower);
        tower.GetNode<Polygon2D>("Polygon2D").Visible = false;

        GlobalPosition = StartingPosition;
    }

    public override void _Process(double delta)
    {
        if (IsHeld)
        {
            Position = GetGlobalMousePosition() - offset;
            // GlobalPosition = map.MapToLocal(map.LocalToMap(GetGlobalMousePosition() - offset));
            // GD.Print(GlobalPosition, offset);
        }
    }
}