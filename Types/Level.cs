using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
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
        public List<Vector2> Obstacles;
        [JsonInclude]
        public double StartingCredits;
        [JsonInclude]
        public int MaxHP;
        [JsonInclude]
        public List<Wave> Waves;
        [JsonInclude]
        public string[] AvailableTowers;
        [JsonIgnore]
        public string Filename;
        public Level Clone()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new Vector2Converter());
            options.Converters.Add(new Vector2IConverter());
            options.Converters.Add(new EnemyConverter());
            options.TypeInfoResolver = SourceGenerationContext.Default;

            return JsonSerializer.Deserialize<Level>(JsonSerializer.Serialize(this, options), options);
        }
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