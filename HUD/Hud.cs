using Godot;
using System;

public partial class Hud : Node2D
{
    public Label TimerText;
    public Label WaveText;

    private int _currentWave;
    private int _maxWaves;

    public override void _Ready()
    {
        TimerText = GetNode<Label>("TimerText");
        WaveText = GetNode<Label>("WaveText");
    }

    public void SetWaves(int current, int max)
    {
        _currentWave = current;
        _maxWaves = max;

        WaveText.Text = "Wave " + _currentWave + "/" + _maxWaves;
    }

    public void SetTimer(int seconds)
    {
        
    }

}
