using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Godot;

namespace FOSSGames
{
    public class Level
    {
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
}