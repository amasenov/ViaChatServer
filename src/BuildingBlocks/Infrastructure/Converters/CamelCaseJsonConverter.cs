using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Converters
{
    public class CamelCaseJsonConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        /// <summary>
        /// Read JSON and return a TEnum type.
        /// </summary>
        public override TEnum Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            return Enum.TryParse(reader.GetString() ?? string.Empty, true, out TEnum result) ? result : default;
        }
        /// <summary>
        /// Write the value to JSON.
        /// </summary>
        public override void Write(
            Utf8JsonWriter writer,
            TEnum value,
            JsonSerializerOptions options
        )
        {
            writer.WriteStringValue(value.ToString().ToLowerFirstCharacter());
        }
    }
}
