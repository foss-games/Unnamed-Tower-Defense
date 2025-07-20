using System;
using Godot;

public partial class UpgradeArea : Node2D
{
    public Label label;
    private Tower target;
    private Button button;
    public override void _Ready()
    {
        label = GetNode<Label>("Label");
        button = GetNode<Button>("Button");
        button.Pressed += UpgradeTower;
    }

    public void Show(Tower tower)
    {
        tower.LevelChanged += () =>
        {
            Show(target);
        };
        target = tower;

        Purchasable sprite = GetNode<Purchasable>("Purchasable");
        sprite.Init(tower.TowerType.GUID.ToString());
        sprite.Turret.Frame = target.TurretFrame;

        label.Text = $"Level {tower.Level}\n";

        if (tower.Level > tower.TowerType.Upgrades.Length)
        {
            label.Text += "No Upgrades Availalbe";
            button.Visible = false;
        }
        else
        {
            label.Text += $"Upgrade ${tower.TowerType.Upgrades[tower.Level - 1].Cost}";
            button.Visible = true;
        }

        Visible = true;
    }
    public void UpgradeTower()
    {
        target.Upgrade();
    }
}
