using System.Text.Json.Serialization;
using Godot;

namespace FOSSGames
{
    public class Sprite
    {
        [JsonInclude]
        public string SpriteSheet;
        [JsonInclude]
        public double Scale;

    }
    public class Attack
    {
        [JsonInclude]
        public double Damage;
        [JsonInclude]
        public double Range;
        [JsonInclude]
        public int Cooldown;
        [JsonInclude]
        public AttackTypes Type;
        [JsonInclude]
        public AttackDetails AttackDetails;
    }

    public class AttackDetails
    {
        [JsonInclude]
        public Sprite Sprite;
        [JsonInclude]
        public double TravelSpeed;
    }

    public class ProjectileAttackDetails : AttackDetails
    {
        //
    }

    public class MissileAttackDetails : AttackDetails
    {
        //
    }

    public enum AttackTypes
    {
        Projectile,
        Missile
    }

    public class Upgrade
    {
        [JsonInclude]
        public double Cost;
        [JsonInclude]
        public UpgradeEffect[] UpgradeEffects;
        [JsonInclude]
        public Vector2[]? Markers;
    }

    public class UpgradeEffect
    {
        [JsonInclude]
        public UpgradeEffectEffects Effect;
        [JsonInclude]
        public double Value;
    }

    public enum UpgradeEffectEffects
    {
        Damage,
        DamageMult,
        RateOfFire,
        RateOfFireMult,
        TargetingRange,
        TargetingRangeMult,
    }

    public class Tower
    {
        [JsonInclude]
        public Sprite Sprite;
        [JsonInclude]
        public double HP;
        [JsonInclude]
        public double VisionRange;
        [JsonInclude]
        public double Cost;
        [JsonInclude]
        public Vector2[] Markers;
        [JsonInclude]
        public Attack[] Attacks;
        [JsonInclude]
        public Upgrade[] Upgrades;
    }
}