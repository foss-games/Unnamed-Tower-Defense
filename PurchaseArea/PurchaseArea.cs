using FOSSGames;
using Godot;

public partial class PurchaseArea : Node2D
{
    private Play play;
    private GridContainer container;
    private PackedScene purcahsable = GD.Load<PackedScene>("res://PurchaseArea/Purchasable.tscn");
    public override void _Ready()
    {
        play = (Play)GetTree().GetFirstNodeInGroup("play");
        container = GetNode<GridContainer>("PurchasableTowers");

        foreach (FOSSGames.Tower t in Global.Instance.Towers)
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
