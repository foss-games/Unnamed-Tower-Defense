using Godot;
using System;

public partial class CurrentWave : Label
{
    public int Total = 0;
    private int _current = 0;
    public int Current
    {
        get
        {
            return _current;
        }
        set
        {
            if (_current == Total)
            {
                //cannot increment
            }
            _current++;
            UpdateText();
        }
    }

    public override void _Ready()
    {
        Text = "Wave " + _current.ToString() + "/" + Total.ToString();
    }

    public void UpdateText()
    {
        Text = "Wave " + _current.ToString() + "/" + Total.ToString();
    }

    public void SetMax(int maxWaves)
    {
        Total = maxWaves;
        UpdateText();
    }

}
