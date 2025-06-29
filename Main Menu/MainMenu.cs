using FOSSGames;
using Godot;

public partial class MainMenu : Node2D
{
    private bool levelSelectVisible = false;
    private GridContainer gridContainer;
    public override void _Ready()
    {
        gridContainer = GetNode<GridContainer>("GridContainer");
        //create buttons and add to GridContainer
        for (int i = 0; i < Global.Instance.Levels.Count; i++)
        {
            CustomDataButton button = new CustomDataButton();
            button.CustomData = i;
            button.PressedWithData += OnLevelSelection;
            button.Text = $"Level {i + 1}";
            button.AddThemeFontSizeOverride("font_size", 30);
            gridContainer.AddChild(button);
        }

        if (Global.Instance.Debug)
        {
            GetNode<Button>("MapMakerButton").Visible = true;
        }
    }

    public void OnPlayButton()
    {
        GetNode<Label>("Title").Text = levelSelectVisible ? "Tower Defense!" : "Choose Level!";
        GetNode<Button>("PlayButton").Visible = levelSelectVisible;
        GetNode<Button>("MapMakerButton").Visible = levelSelectVisible;
        GetNode<GridContainer>("GridContainer").Visible = !levelSelectVisible;
        GetNode<Button>("BackButton").Visible = !levelSelectVisible;
        levelSelectVisible = !levelSelectVisible;
    }
    public void OnMapButton()
    {
        GetTree().ChangeSceneToFile("res://LevelMaker/LevelMaker.tscn");
    }
    public void OnLevelSelection(Variant level)
    {
        Global.Instance.SelectedLevelIndex = level.AsInt32();
        GetTree().ChangeSceneToFile("res://Play/Play.tscn");
    }
}