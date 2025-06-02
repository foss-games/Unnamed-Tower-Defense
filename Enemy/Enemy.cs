using Godot;
using System;

public partial class Enemy : Node2D
{
    private int _hp;
    public int HP
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
            if (_hp == 0) Die();
        }
    }

    public int Reward;
    public int Speed;

    Play play;

    public override void _Ready()
    {
        play = (Play)GetTree().GetFirstNodeInGroup("play");
    }


    public void Die()
    {
        //do death effect

        //credit player with reward
        play.Credits += Reward;
        
        GD.Print("I am ded.");
        Visible = false;
        //destroy object
        QueueFree();
    }
}
