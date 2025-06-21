
using System;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace FOSSGames
{
    public class ColorConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Color c = new Color();
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
                        case "r":
                            c.R = reader.GetSingle();
                            break;
                        case "g":
                            c.G = reader.GetSingle();
                            break;
                        case "b":
                            c.B = reader.GetSingle();
                            break;
                        case "a":
                            c.A = reader.GetSingle();
                            break;
                    }
                    reader.Read();
                    count++;
                }
            }
            return c;
        }
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{{R: {value.R}, G: {value.G}, B: {value.B}, A: {value.A}}}");
        }
    }
}