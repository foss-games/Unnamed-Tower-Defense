using System;
using Godot;

public partial class MainMenu : Node2D
{
    public void OnPlayButton()
    {
        GetTree().ChangeSceneToFile("res://Play/Play.tscn");
    }
    public void OnMapButton()
    {
        GetTree().ChangeSceneToFile("res://LevelMaker/LevelMaker.tscn");
    }
}
