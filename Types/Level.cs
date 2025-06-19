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
        public double StartingCredits;
        public int MaxHP;
        public PoppableList<Wave> Waves;
    }

    public class Wave
    {
        public int Interval;
        public float TimeSinceLastSpawn = 0;
        public int SpawnedCount = 0;
        public List<WaveEnemies> Enemies;

    }

    public class WaveEnemies
    {
        public Enemy Enemy;
        public int Count;
        public float Interval = 1.0f;
    }
}