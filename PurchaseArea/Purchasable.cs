
using FOSSGames;
using Godot;

public partial class Purchasable : Control
{
    public FOSSGames.Tower Tower;
    private Play play;

    PackedScene towerScene = GD.Load<PackedScene>("res://Tower/Tower.tscn");

    public void Init(string guid)
    {
        Tower = Global.Instance.Towers.Find(t => t.GUID.ToString() == guid);
        play = (Play)GetTree().GetFirstNodeInGroup("play");

        Button button = GetNode<Button>("Button");
        button.ButtonDown += TowerPickedUp;
        Node2D container = button.GetNode<Node2D>("Node2D");
        Label label = GetNode<Label>("Label");
        Sprite2D turret = container.GetNode<Sprite2D>("Turret");
        Sprite2D body = container.GetNode<Sprite2D>("Body");

        body.Texture = new AtlasTexture
        {
            Atlas = GD.Load<CompressedTexture2D>("res://Resources/towers/TowerTileSet.png"),
            Region = new Rect2(0, Tower.Sprite.Frame * 32, 0, 0)
        };

        turret.Texture = new AtlasTexture
        {
            Atlas = GD.Load<CompressedTexture2D>("res://Resources/towers/TowerTileSet.png"),
            Region = new Rect2(32, Tower.Sprite.Frame * 32, 0, 0)
        };

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
