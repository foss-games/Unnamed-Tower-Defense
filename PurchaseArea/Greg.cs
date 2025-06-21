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
    private Vector2I offset = new Vector2I(0, -60);
    private Label debugLabel;

    public override void _Ready()
    {
        StartingPosition = GlobalPosition;
        map = GetTree().GetFirstNodeInGroup("background").GetNode<TileMapLayer>("TileMapLayer");
        towerCollection = (Node2D)GetTree().GetFirstNodeInGroup("towerscollection");
        tower = GetNode<Tower>("Tower");
        tower.AIEnabled = false;
        button = GetNode<Button>("Button");
        button.ButtonDown += TowerPickedUp;
        button.ButtonUp += TowerDropped;
        play = (Play)GetTree().GetFirstNodeInGroup("play");
        debugLabel = GetNode<Label>("debug");
    }

    public void TowerPickedUp()
    {
        if (play.Credits < tower.Cost)
        {
            return;
        }
        if (!IsHeld)
        {
            IsHeld = true;
            tower.GetNode<Sprite2D>("Circle").Visible = true;
        }
    }

    public void TowerDropped()
    {
        if (!IsHeld) return;
        IsHeld = false;
        Tower newTower = (Tower)towerScene.Instantiate();
        newTower.CallDeferred("Move", map.MapToLocal(map.LocalToMap(GetGlobalMousePosition() + offset)));
        towerCollection.AddChild(newTower);
        tower.GetNode<Sprite2D>("Circle").Visible = false;

        GlobalPosition = StartingPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsHeld)
        {
            GlobalPosition = GetGlobalMousePosition() + offset;
            QueueRedraw();
        }
        Vector2 mouse = GetGlobalMousePosition();
        // debugLabel.GlobalPosition = new Vector2(mouse.X - 50, mouse.Y - 10);
        // debugLabel.Text = map.LocalToMap(mouse).ToString() + "\n" + mouse.ToString();
    }
}