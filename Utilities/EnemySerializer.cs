using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace FOSSGames
{
    public class EnemyConverter : JsonConverter<Enemy>
    {
        public override Enemy Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Enemy e = new Enemy();

            //GD.Print(JsonDocument.ParseValue(ref reader).RootElement.GetRawText());

            JsonSerializerOptions voptions = new JsonSerializerOptions();
            voptions.Converters.Add(new Vector2Converter());
            voptions.Converters.Add(new Vector2IConverter());
            voptions.Converters.Add(new ColorConverter());
            voptions.Converters.Add(new EnemyUpgradeConverter());
            voptions.Converters.Add(new SpriteConverter());

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    _ = reader.Read();
                    continue;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string prop = reader.GetString();
                    reader.Read();

                    switch (prop.ToLower())
                    {
                        case "guid":
                            string rawGUID = reader.GetString();
                            e.GUID = new Guid(rawGUID);
                            break;
                        case "sprite":
                            string rawSprite = JsonDocument.ParseValue(ref reader).RootElement.GetRawText();
                            e.Sprite = JsonSerializer.Deserialize<Sprite>(rawSprite, voptions);
                            break;
                        case "hp":
                            e.HP = reader.GetDouble();
                            break;
                        case "speed":
                            e.Speed = reader.GetDouble();
                            break;
                        case "reward":
                            e.Reward = reader.GetDouble();
                            break;
                        case "upgrade":
                            string rawUpgrade = JsonDocument.ParseValue(ref reader).RootElement.GetRawText();
                            e.Upgrade = JsonSerializer.Deserialize<EnemyUpgrade>(rawUpgrade, voptions);
                            break;
                    }
                    reader.Read();
                }
            }
            return e;
        }
        public override void Write(Utf8JsonWriter writer, Enemy value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{value}");
        }
    }
    public class EnemyUpgradeConverter : JsonConverter<EnemyUpgrade>
    {
        public override EnemyUpgrade Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            EnemyUpgrade upgrade = new EnemyUpgrade();
            int count = 0;
            while (count < 4)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    _ = reader.Read();
                    continue;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string prop = reader.GetString();
                    reader.Read();

                    switch (prop.ToLower())
                    {
                        case "speed":
                            upgrade.Speed = reader.GetSingle();
                            break;
                        case "speedmult":
                            upgrade.SpeedMult = reader.GetSingle();
                            break;
                        case "hp":
                            upgrade.HP = reader.GetSingle();
                            break;
                        case "hpmult":
                            upgrade.HPMult = reader.GetSingle();
                            break;
                    }
                    reader.Read();
                    count++;
                }
            }
            return upgrade;
        }
        public override void Write(Utf8JsonWriter writer, EnemyUpgrade value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{{HP: {value.HP}, HPMult: {value.HPMult}, Speed: {value.Speed}, SpeedMult: {value.SpeedMult}}}");
        }
    }
}