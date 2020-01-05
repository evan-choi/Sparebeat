using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sparebeat.Json
{
    class Int32Converter : JsonConverter<int>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(int);
        }

        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
                return int.Parse(reader.GetString());

            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetInt32();

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
