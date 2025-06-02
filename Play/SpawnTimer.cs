using Godot;
using System;

public partial class SpawnTimer : Timer
{

    DateTime NextAction;
    Play play;

    public override void _Ready()
    {
        WaitTime = 0.25;
        play = (Play)GetTree().GetFirstNodeInGroup("play");
    }

    public void OnTimerTimeout()
    {
        if ((NextAction - new DateTime()).Milliseconds <= 0)
        {
            play.NextSpawn();
        }
    }

    public void Set(double seconds)
    {
        NextAction =  new DateTime().AddSeconds(seconds);
    }
}
