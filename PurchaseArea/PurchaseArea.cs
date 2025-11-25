using System.Linq;
using FOSSGames;
using Godot;

public partial class PurchaseArea : Node2D
{
    private Play play;
    private GridContainer container;
    private PackedScene purcahsable = GD.Load<PackedScene>("res://PurchaseArea/Purchasable.tscn");
    public override void _Ready()
    {
        container = GetNode<GridContainer>("PurchasableTowers");
        play = (Play)GetTree().GetFirstNodeInGroup("play");
        //bypass for level builder
        if (play == null) return;
        Level level = Global.Instance.Levels[Global.Instance.SelectedLevelIndex];

        foreach (FOSSGames.Tower t in Global.Instance.Towers.FindAll(t => level.AvailableTowers.Contains(t.GUID.ToString())))
        {
            Purchasable p = purcahsable.Instantiate<Purchasable>();
            p.CallDeferred("Init", t.GUID.ToString());
            container.Columns++;
            container.AddChild(p);
        }
        container.AddChild(new Control()
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        });
    }

}
