using Godot;
using System;

public partial class SpawnTimer : Timer
{
    public int Seconds;
    public int Minutes;
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
