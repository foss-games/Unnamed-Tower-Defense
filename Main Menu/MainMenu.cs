using System;
using System.Text.Json;
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
            CustomDataButton button = new()
            {
                CustomData = i,
                Text = $"Level {i + 1}"
            };
            button.PressedWithData += OnLevelSelection;
            button.AddThemeFontSizeOverride("font_size", 30);
            gridContainer.AddChild(button);
        }

        if (Global.Instance.Debug)
        {
            GetNode<Button>("MapMakerButton").Visible = true;
        }

        //string json = JsonSerializer.Serialize<Level>(Global.Instance.Levels[0], SourceGenerationContext.Default.Level);

        // Wave w = new Wave();
        // w.Interval = 3;
        // w.Enemies = [];
        // w.Enemies.Add(new WaveEnemy() { GUID = Guid.NewGuid() });
        // w.Enemies.Add(new WaveEnemy() { GUID = Guid.NewGuid() });

        //GD.Print(Json.Stringify(w));

        // GD.Print(w.ToJSON());
        return;
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