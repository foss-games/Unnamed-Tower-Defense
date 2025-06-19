using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace FOSSGames
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Vector2 v = new Vector2();
            int count = 0;
            while (count < 2)
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

                    if (prop.ToLower() == "x")
                    {
                        v.X = reader.GetSingle();
                        reader.Read();
                        count++;
                        continue;
                    }
                    if (prop.ToLower() == "y")
                    {
                        v.Y = reader.GetSingle();
                        reader.Read();
                        count++;
                        continue;
                    }
                }
            }
            return v;
        }
        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{{X: {value.X}, Y: {value.Y}}}");
            //throw new NotImplementedException();
        }
    }

    public class Vector2IConverter : JsonConverter<Vector2I>
    {
        public override Vector2I Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Vector2I v = new Vector2I();
            int count = 0;
            while (count < 2)
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

                    if (prop.ToLower() == "x")
                    {
                        v.X = reader.GetInt32();
                        reader.Read();
                        count++;
                        continue;
                    }
                    if (prop.ToLower() == "y")
                    {
                        v.Y = reader.GetInt32();
                        reader.Read();
                        count++;
                        continue;
                    }
                }
            }
            return v;
        }
        public override void Write(Utf8JsonWriter writer, Vector2I value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{{X: {value.X}, Y: {value.Y}}}");
            //throw new NotImplementedException();
        }
    }
}