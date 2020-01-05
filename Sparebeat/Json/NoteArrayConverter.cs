using Sparebeat.Common;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sparebeat.Json
{
    class INoteConverter : JsonConverter<INote>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(INote);
        }

        public override INote Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
                return new Note(reader.GetString());

            return JsonSerializer.Deserialize<NoteProperty>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, INote value, JsonSerializerOptions options)
        {
            if (value is Note note)
            {
                writer.WriteStringValue(note.Data);
            }
            else
            {
                JsonSerializer.Serialize(writer, value);
            }
        }
    }
}
