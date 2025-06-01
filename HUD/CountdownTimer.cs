using Godot;
using System;

public partial class CountdownTimer : Timer
{
    public int Seconds = 10;
    public int Minutes = 0;
    private Label label;

    private Play play;

    public override void _Ready()
    {
        label = GetParent<Label>();

        play = (Play)GetTree().GetFirstNodeInGroup("play");

        label.Text = TimeToString();
        Decrement();
    }

    public void OnTimerTimeout()
    {
        label.Text = TimeToString();

        if (Minutes == 0 && Seconds < 10)
        {
            label.Set("theme_override_colors/font_color", Color.Color8(255, 0, 0, 255));
        }

        if (Seconds == 0 && Minutes == 0)
        {
            play.Call("NextWave");
            label.Set("theme_override_colors/font_color", Color.Color8(255, 255, 255, 255));
            return;
        }

        Decrement();
        
    }

    public void Set(int interval)
    {
        GD.Print("CountdownTimer.Set");
        Seconds = interval;
    }

    public void Decrement()
    {
        if (Seconds < 1 && Minutes < 1)
        {
            throw new Exception("Invalid Time, Unable to decrement.");
        }

        if (Seconds < 1)
        {
            Minutes--;
            Seconds = 59;
            return;
        }

        Seconds--;
    }

    public string TimeToString()
    {
        string seconds = Seconds.ToString();
        string minutes = Minutes.ToString();
        //prepend a 0 if required
        if (Seconds < 10) seconds = "0" + seconds;
        if (Minutes < 10) minutes = "0" + minutes;

        return minutes + ":" + seconds;
    }
}
