using System;
using System.Collections.Generic;
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

        //load levels
        List<Level> levels = LoadLevels();
        //create buttons and add to GridContainer

        for (int i = 0; i < levels.Count; i++)
        {
            CustomDataButton button = new CustomDataButton();
            button.CustomData = i;
            button.PressedWithData += OnLevelSelection;
            button.Text = $"Level {i}";
            button.AddThemeFontSizeOverride("font_size", 30);
            gridContainer.AddChild(button);
        }
    }

    public List<Level> LoadLevels()
    {
        List<Level> levels = [];
        using DirAccess dir = DirAccess.Open("res://Resources/Levels/");
        if (dir == null) throw new Exception("Unable to load levels.");

        JsonSerializerOptions options = new JsonSerializerOptions();
        options.Converters.Add(new Vector2Converter());
        options.Converters.Add(new Vector2IConverter());
        options.Converters.Add(new EnemyConverter());

        foreach (string filename in dir.GetFiles())
        {
            if (!filename.EndsWith("json")) continue;
            string json = FileAccess.Open("res://Resources/Levels/" + filename, FileAccess.ModeFlags.Read).GetAsText();

            levels.Add(JsonSerializer.Deserialize<FOSSGames.Level>(json, options));
        }
        return levels;
    }

    public void OnPlayButton()
    {
        //GetTree().ChangeSceneToFile("res://Play/Play.tscn");
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
        GD.Print(level.AsInt32());
    }
}