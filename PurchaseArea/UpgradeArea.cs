using System;
using Godot;

public partial class UpgradeArea : Node2D
{
    public Label label;
    public override void _Ready()
    {
        label = GetNode<Label>("Label");
    }

    public void Show(Tower tower)
    {
        Purchasable sprite = GetNode<Purchasable>("Purchasable");
        sprite.Init(tower.TowerType.GUID.ToString(), false);

        label.Text = $"Level {tower.Level}\n";

        if (tower.Level > tower.TowerType.Upgrades.Length)
        {
            label.Text += "No Upgrades Availalbe";
        }
        else
        {
            label.Text += $"Upgrade ${tower.TowerType.Upgrades[tower.Level - 1].Cost}";
        }

        Visible = true;
    }
}
