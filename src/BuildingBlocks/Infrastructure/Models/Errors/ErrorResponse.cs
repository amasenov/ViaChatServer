using System;
using System.Text.Json.Serialization;
using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Models
{
    /// <summary>
    /// The model that is returned in case of an error.
    /// </summary>
    public record ErrorResponse
    {
        public ErrorResponse(string key, string message)
        {
            if (key.IsEmpty())
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (message.IsEmpty())
            {
                throw new ArgumentNullException(nameof(message));
            }

            Key = key;
            Message = message;
        }

        /// <summary>
        /// Gets the key of the error.
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets the message of the error.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets the optional additional data.
        /// </summary>
        [JsonPropertyName("additionalData")]
        public object AdditionalData { get; set; }
    }
}
