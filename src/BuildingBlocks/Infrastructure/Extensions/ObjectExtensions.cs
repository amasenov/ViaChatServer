using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

using ViaChatServer.BuildingBlocks.Infrastructure.Constants;
using ViaChatServer.BuildingBlocks.Infrastructure.Resources;
using ViaChatServer.BuildingBlocks.Infrastructure.Serializing;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object value) => JsonSerializer.Serialize(value, SerializingSettings.SerializerOptions);

        public static StringContent ToStringContent(this object content) => content.ToJson().ToStringContent();

        public static bool HasValue(this object obj) => obj != null;
        public static bool IsDefault(this object obj) => obj == default;
        public static bool IsNull(this object obj) => obj == null;
        public static T GetPropertyValue<T>(this object obj, string propName) => (T)obj.GetType().GetProperty(propName)?.GetValue(obj, null) ?? throw new ArgumentOutOfRangeException(nameof(propName));
        public static void ValidateModel(this object obj)
        {
            ValidationContext context = new(obj, serviceProvider: null, items: null);
            List<ValidationResult> validationResults = new();

            bool isValid = Validator.TryValidateObject(obj, context, validationResults, true);
            if(!isValid)
            {
                object additionalData = new
                {
                    detailedErrorMessages = validationResults.SelectMany(x => x.MemberNames.Select(k => new { key = k, value = x.ErrorMessage }) ).ToList()
                };

                throw new Exceptions.ValidationException(ApiDefinitions.GenericValidationKey, ModelValidation.GenericModelValidation, additionalData);
            }
        }
    }
}
