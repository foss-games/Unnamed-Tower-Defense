using Godot;
using System;

public partial class Hud : Node2D
{
    public Label TimerText;
    public Label WaveText;

    private int _currentWave;
    private int _maxWaves;

    private int _minutes = 0;
    private int _seconds = 0;

    private Timer timer;

    public override void _Ready()
    {
        TimerText = GetNode<Label>("TimerText");
        WaveText = GetNode<Label>("WaveText");
        timer = GetNode<CountdownTimer>("HudTimer");
    }

    public void SetWaves(int current, int max)
    {
        _currentWave = current;
        _maxWaves = max;

        WaveText.Text = "Wave " + _currentWave + "/" + _maxWaves;
    }

    public void SetTimer(int seconds)
    {
        _seconds = seconds % 60;
        _minutes = seconds / 60; //_minutes is int, so the decimal portion is lost automatically
        UpdateTimerText();
    }

    public void UpdateTimerText()
    {
        TimerText.Text = _minutes.ToString("D2") + ":" + _seconds.ToString("D2");
    }

    public void OnTimerTimeout()
    {
        if (_seconds == 0 && _minutes == 0)
        {
            timer.Stop();
            return;
        }

        if (_seconds == 0)
        {
            _seconds = 59;
            _minutes--;
            return;
        }

        _seconds--;
        UpdateTimerText();
    }
}
