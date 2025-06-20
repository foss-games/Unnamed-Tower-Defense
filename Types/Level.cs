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
    }

    public class Wave
    {
        [JsonInclude]
        public int Interval;
        [JsonInclude]
        public float TimeSinceLastSpawn = 0;
        [JsonInclude]
        public int SpawnedCount = 0;
        [JsonInclude]
        public List<WaveEnemies> Enemies;

    }

    public class WaveEnemies
    {
        [JsonInclude]
        public Enemy Enemy;
        [JsonInclude]
        public int Count;
        [JsonInclude]
        public float Interval = 1.0f;
    }
}