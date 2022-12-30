using Microsoft.AspNetCore.Mvc;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Serializing
{
    public static class SerializingSettings
    {
        public static JsonSerializerOptions SerializerOptions
        {
            get
            {
                JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web)
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                serializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

                return serializerOptions;
            }
        }

        public static void JsonOptions(JsonOptions options)
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

            // Configure a custom converter.
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }
    }
}
