using System;
using System.ComponentModel.DataAnnotations.Schema;
using Godot;

public partial class Greg : Node2D
{
    public int Speed = 50;
    public bool HasMouse = false;
    public bool IsHeld = false;
    public Button button;

    public Vector2 StartingPosition;
    PackedScene towerScene = GD.Load<PackedScene>("res://Tower/Tower.tscn");
    private TileMapLayer map;
    private Node2D towerCollection;
    private Tower tower;
    private Play play;

    public override void _Ready()
    {
        StartingPosition = GlobalPosition;
        map = (TileMapLayer)GetTree().GetFirstNodeInGroup("background");
        towerCollection = (Node2D)GetTree().GetFirstNodeInGroup("towerscollection");
        tower = GetNode<Tower>("Tower");
        tower.AIEnabled = false;
        button = GetNode<Button>("Button");
        button.ButtonDown += TowerPickedUp;
        button.ButtonUp += TowerDropped;
        play = (Play)GetTree().GetFirstNodeInGroup("play");
    }

    public void TowerPickedUp()
    {
        GD.Print(play.Credits);
        GD.Print(tower.Cost);
        if (play.Credits < tower.Cost)
        {
            GD.Print("No");
            return;
        }
        if (!IsHeld)
        {
            GD.Print("Grabbed");
            IsHeld = true;
            tower.GetNode<Sprite2D>("Circle").Visible = true;
        }
    }

    public void TowerDropped()
    {
        if (!IsHeld) return;
        GD.Print("Dropped");
        //GD.Print(map.MapToLocal(map.LocalToMap(GetGlobalMousePosition())));
        IsHeld = false;
        Tower newTower = (Tower)towerScene.Instantiate();
        newTower.CallDeferred("Move", map.MapToLocal(map.LocalToMap(GetGlobalMousePosition())));
        towerCollection.AddChild(newTower);
        tower.GetNode<Sprite2D>("Circle").Visible = false;

        GlobalPosition = StartingPosition;
        //GD.Print(newTower.Cost);
        ((Play)GetTree().GetFirstNodeInGroup("play")).Credits -= newTower.Cost;
    }

    public override void _Process(double delta)
    {
        if (IsHeld)
        {
            GlobalPosition = GetGlobalMousePosition();
            QueueRedraw();
        }
    }
}