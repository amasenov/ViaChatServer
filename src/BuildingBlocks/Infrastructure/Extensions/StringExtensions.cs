using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

using ViaChatServer.BuildingBlocks.Infrastructure.Serializing;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string ToLowerFirstCharacter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var firstCharacter = char.ToLowerInvariant(input[0]);
            string otherCharacters = string.Empty;
            if (input.Length > 1)
            {
                otherCharacters = input[1..];
            }
            return $"{firstCharacter}{otherCharacters}";
        }
        public static bool IsEqual(this string first, string second, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase) => first.HasValue() && second.HasValue() && first.Trim().Equals(second.Trim(), comparisonType);
        public static bool IsContaining(this string first, string second, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase) => first.HasValue() && second.HasValue() && first.Trim().Contains(second.Trim(), comparisonType);
        public static T FromJson<T>(this string message) where T : new() => JsonSerializer.Deserialize<T>(message, SerializingSettings.SerializerOptions);
        public static StringContent ToStringContent(this string content) => new(content, Encoding.UTF8, MediaTypeNames.Application.Json);
        public static bool HasValue(this string value) => !value.IsEmpty();
        public static bool IsEmpty(this string value) => string.IsNullOrWhiteSpace(value);
    }
}
