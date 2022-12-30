using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ViaChatServer.BuildingBlocks.Infrastructure.Constants;
using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Converters
{
    /// <summary>
    /// A class for generic converting enum with flags to and from JSON.
    /// </summary>
	public class EnumFlagsJsonConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum, IConvertible
    {
        /// <summary>
        /// Read JSON and return a CommunicationType.
        /// </summary>
        public override TEnum Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            TEnum result = default;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return result;
                }
                if (reader.TokenType != JsonTokenType.String)
                {
                    throw new JsonException();
                }

                if (Enum.TryParse(reader.GetString(), true, out TEnum flag))
                {
                    result = (TEnum)(object)(GetInt(result) | GetInt(flag));
                }
            }

            throw new JsonException();
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
            if (GetInt(value) == GetInt(default))
            {
                // the 0 value is special, as it would always trigger with the code below
                writer.WriteStartArray();
                writer.WriteStringValue(default(TEnum).ToString().ToLowerFirstCharacter());
                writer.WriteEndArray();
                return;
            }
            writer.WriteStartArray();
            foreach (var name in Enum.GetNames(typeof(TEnum)))
            {
                if (Enum.TryParse(name, true, out TEnum flag) && (GetInt(flag) != GetInt(default)) && value.HasFlag(flag))
                {
                    writer.WriteStringValue(name.ToLowerFirstCharacter());
                }
            }
            writer.WriteEndArray();
        }

        private static int GetInt(TEnum value) => value.ToInt32(LocalizationValues.EnglishCulture);
    }
}