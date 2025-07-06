using System;
using Godot;

namespace FOSSGames
{
    public struct Enemy
    {
        public Guid GUID;
        public Sprite Sprite;
        public double HP;
        public double Speed;
        public double Reward;
        public EnemyUpgrade Upgrade;
    }
    public enum EnemyStates
    {
        Enabled,
        Disabled,
        Celebrating,
    }
}
