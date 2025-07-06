using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using Godot.Collections;

namespace FOSSGames
{
    public partial class Global : Node
    {
        //globally availalbe properties
        public int SelectedLevelIndex;
        public bool Debug = true;
        public List<Enemy> EnemyTypes = [];
        public List<Level> Levels = [];
        public List<Tower> Towers = [];
        public Array<string> CompletedLevels = new Array<string>();

        //Load resources
        public void LoadEnemies()
        {
            using DirAccess dir = DirAccess.Open("res://Resources/Enemies/");
            if (dir == null) throw new Exception("Unable to load enemies.");

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new Vector2Converter());
            options.Converters.Add(new Vector2IConverter());
            options.Converters.Add(new EnemyConverter());
            options.TypeInfoResolver = SourceGenerationContext.Default;

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
            options.TypeInfoResolver = SourceGenerationContext.Default;


            foreach (string filename in dir.GetFiles())
            {
                if (!filename.EndsWith("json")) continue;
                string json = FileAccess.Open($"res://Resources/Levels/{filename}", FileAccess.ModeFlags.Read).GetAsText();
                Level level = JsonSerializer.Deserialize<Level>(json, options);
                level.Filename = filename;
                Levels.Add(level);
            }
        }

        public void LoadTowers()
        {
            using DirAccess dir = DirAccess.Open("res://Resources/Towers/");
            if (dir == null) throw new Exception("Unable to load towers.");

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new Vector2Converter());
            options.Converters.Add(new Vector2IConverter());
            options.Converters.Add(new EnemyConverter());
            options.TypeInfoResolver = SourceGenerationContext.Default;

            foreach (string filename in dir.GetFiles())
            {
                if (!filename.EndsWith("json")) continue;
                string json = FileAccess.Open($"res://Resources/Towers/{filename}", FileAccess.ModeFlags.Read).GetAsText();
                Tower tower = JsonSerializer.Deserialize<Tower>(json, options);
                tower.Filename = filename;
                Towers.Add(tower);
            }

            Towers.Sort((a, b) => a.Sort > b.Sort ? 1 : -1);
        }

        public static void SaveGame()
        {
            using FileAccess saveFile = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);

            var data = new Godot.Collections.Dictionary<string, Variant>
            {
                { "CompletedLevels", Instance.CompletedLevels }
            };

            saveFile.StoreLine(Json.Stringify(data));
        }

        public static void LoadSaveGame()
        {
            if (!FileAccess.FileExists("user://savegame.save"))
            {
                //No save to load
                return;
            }
            using FileAccess saveFile = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
            var line = saveFile.GetLine();
            Godot.Collections.Dictionary<string, Variant> parsed = (Godot.Collections.Dictionary<string, Variant>)Json.ParseString(line);

            Instance.CompletedLevels = (Array<string>)parsed["CompletedLevels"];

            return;

        }

        //Global's singleton setup
        private static Global _instance;
        public static Global Instance => _instance;
        public override void _EnterTree()
        {
            if (_instance != null) QueueFree();
            _instance = this;

            LoadEnemies();
            LoadLevels();
            LoadTowers();
            LoadSaveGame();
        }

    }
}