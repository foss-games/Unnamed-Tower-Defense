using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Godot;

namespace FOSSGames
{
    public class Level
    {
        [JsonInclude]
        public Guid GUID;
        [JsonInclude]
        public Vector2 StartLocation;
        [JsonInclude]
        public Vector2 EndLocation;
        [JsonInclude]
        public Vector2[] Obstacles;
        [JsonInclude]
        public double StartingCredits;
        [JsonInclude]
        public int MaxHP;
        [JsonInclude]
        public List<Wave> Waves;
        [JsonInclude]
        public string[] AvailableTowers;
        public string Filename;
    }

    public class Wave
    {
        [JsonInclude]
        public int Interval;
        public float TimeSinceLastSpawn = 0;
        public int SpawnedCount = 0;
        public int EnemySet = 0;
        [JsonInclude]
        public List<WaveEnemy> Enemies;
    }

    public class WaveEnemy
    {
        [JsonInclude]
        public Guid GUID;
        [JsonInclude]
        public int Count;
        [JsonInclude]
        public double Interval;
    }

    public enum LevelState
    {
        Running,
        Complete
    }
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(Level))]
    [JsonSerializable(typeof(WaveEnemy))]
    [JsonSerializable(typeof(Wave))]
    [JsonSerializable(typeof(Enemy))]
    [JsonSerializable(typeof(Tower))]
    [JsonSerializable(typeof(UpgradeEffectEffects))]
    [JsonSerializable(typeof(UpgradeEffect))]
    [JsonSerializable(typeof(Upgrade))]
    [JsonSerializable(typeof(AttackTypes))]
    [JsonSerializable(typeof(AttackDetails))]
    [JsonSerializable(typeof(Attack))]
    [JsonSerializable(typeof(Sprite))]
    internal partial class SourceGenerationContext : JsonSerializerContext { }
}