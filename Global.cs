using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace FOSSGames
{
    public partial class Global : Node
    {
        //globally availalbe properties
        public int SelectedLevelIndex;
        public bool Debug = true;
        public List<Enemy> EnemyTypes = [];
        public List<Level> Levels = [];

        //Global's singleton setup
        private static Global _instance;
        public static Global Instance => _instance;
        public override void _EnterTree()
        {
            if (_instance != null) QueueFree();
            _instance = this;

            LoadEnemies();
            LoadLevels();
        }
        public void LoadEnemies()
        {
            using DirAccess dir = DirAccess.Open("res://Resources/Enemies/");
            if (dir == null) throw new Exception("Unable to load enemies.");

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new Vector2Converter());
            options.Converters.Add(new Vector2IConverter());
            options.Converters.Add(new EnemyConverter());

            foreach (string filename in dir.GetFiles())
            {
                if (!filename.EndsWith("json")) continue;
                string json = FileAccess.Open($"res://Resources/Enemies/{filename}", FileAccess.ModeFlags.Read).GetAsText();

                EnemyTypes.Add(JsonSerializer.Deserialize<FOSSGames.Enemy>(json, options));
            }
        }
        public void LoadLevels()
        {
            using DirAccess dir = DirAccess.Open("res://Resources/Levels/");
            if (dir == null) throw new Exception("Unable to load levels.");

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new Vector2Converter());
            options.Converters.Add(new Vector2IConverter());
            options.Converters.Add(new EnemyConverter());

            foreach (string filename in dir.GetFiles())
            {
                if (!filename.EndsWith("json")) continue;
                string json = FileAccess.Open($"res://Resources/Levels/{filename}", FileAccess.ModeFlags.Read).GetAsText();
                Level level = JsonSerializer.Deserialize<Level>(json, options);
                level.Filename = filename;
                Levels.Add(level);
            }
        }
    }
}