
using FOSSGames;
using Godot;

public partial class Purchasable : Control
{
    public FOSSGames.Tower Tower;
    private Play play;
    public Sprite2D Turret;

    PackedScene towerScene = GD.Load<PackedScene>("res://Tower/Tower.tscn");

    public void Init(string guid)
    {
        Tower = Global.Instance.Towers.Find(t => t.GUID.ToString() == guid);
        play = (Play)GetTree().GetFirstNodeInGroup("play");

        Button button = GetNode<Button>("Button");
        button.ButtonDown += TowerPickedUp;
        Node2D container = button.GetNode<Node2D>("Node2D");
        Label label = GetNode<Label>("Label");
        Turret = container.GetNode<Sprite2D>("Turret");
        Sprite2D body = container.GetNode<Sprite2D>("Body");

        body.Frame = 7 * Tower.Sprite.Frame;
        Turret.Frame = body.Frame + 1;

        container.Modulate = Tower.Sprite.Modulate;
        label.Text = Tower.Cost.ToString();
    }

    public void TowerPickedUp()
    {
        Tower newTower = towerScene.Instantiate<Tower>();
        newTower.State = TowerState.Dragging;
        newTower.GlobalPosition = GetGlobalMousePosition();
        newTower.TowerType = Tower;
        play.GetNode<Node2D>("Towers").AddChild(newTower);
    }
}
