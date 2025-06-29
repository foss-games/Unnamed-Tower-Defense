using Godot;

public partial class EndMenu : Node2D
{
    public override void _Ready()
    {
        GridContainer grid = GetNode<GridContainer>("GridContainer");
        grid.GetNode<Button>("ContinueButton").Pressed += () => GetTree().ChangeSceneToFile("res://Play/Play.tscn");
        grid.GetNode<Button>("MenuButton").Pressed += () => GetTree().ChangeSceneToFile("res://Main Menu/Main Menu.tscn");

        Play play = (Play)GetTree().GetFirstNodeInGroup("play");
        play.LevelLost += () =>
        {
            GetNode<Label>("Label").Text = "Level Lost!";
        };
        play.LevelWon += () =>
        {
            GetNode<Label>("Label").Text = "Congratulations!";
            grid.GetNode<Button>("ContinueButton").Text = "Next Level!";
        };
    }

}
