
using System;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace FOSSGames
{
    public class SpriteConverter : JsonConverter<Sprite>
    {
        public override Sprite Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Sprite sprite = new Sprite();

            JsonSerializerOptions voptions = new JsonSerializerOptions();
            voptions.Converters.Add(new Vector2Converter());
            voptions.Converters.Add(new Vector2IConverter());
            voptions.Converters.Add(new ColorConverter());

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
                        case "frame":
                            sprite.Frame = reader.GetInt32();
                            break;
                        case "scale":
                            //sprite.Scale = reader.get
                            string rawScale = JsonDocument.ParseValue(ref reader).RootElement.GetRawText();
                            sprite.Scale = JsonSerializer.Deserialize<Vector2>(rawScale, voptions);
                            break;
                        case "modulate":
                            string rawModulate = JsonDocument.ParseValue(ref reader).RootElement.GetRawText();
                            sprite.Modulate = JsonSerializer.Deserialize<Color>(rawModulate, voptions);
                            break;
                    }
                    reader.Read();
                }
            }
            return sprite;
        }
        public override void Write(Utf8JsonWriter writer, Sprite value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{{\"Frame\": {value.Frame}, \"Scale\": {value.Scale}, \"Module\": {value.Modulate}}}");
        }
    }
}