using System;
using System.Net;
using System.Net.Http;
using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;
using ViaChatServer.BuildingBlocks.Infrastructure.Models;
using ViaChatServer.BuildingBlocks.Infrastructure.Resources;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Exceptions
{
    /// <summary>
    /// Base class for validation exceptions.
    /// </summary>
    public class ValidationException : HttpRequestException
    {
        public ValidationException(Enum validationError, object additionalData = null, string message = null) : base(validationError.ToString(), null, HttpStatusCode.BadRequest)
        {
            Key = validationError.ToString();
            ValidationError = validationError;
            AdditionalData = additionalData;
            HasCustomMessage = message.HasValue();
            Message = HasCustomMessage ? message : base.Message;

            //if (message.HasValue())
            //{
            //    // Can be used to manually set the exception message
            //    base.Data.Add("message", message);
            //}
        }

        public ValidationException(string key, string message, object additionalData) : base(key, null, HttpStatusCode.BadRequest)
        {
            Key = key;
            Message = message;
            AdditionalData = additionalData;
            HasCustomMessage = true;
        }

        public object AdditionalData { get; }

        public Enum ValidationError { get; }

        public string Key { get; }
        public new string Message { get; }

        public bool HasCustomMessage { get; }

        public static implicit operator ErrorResponse(ValidationException ex)
        {
            string key = ex.Key;
            string message = ex.HasCustomMessage ? ex.Message : ModelValidation.ResourceManager.GetTranslation(ex.ValidationError);
            object additionalData = ex.AdditionalData;

            if ((ex.Data?.Count ?? 0) > 0)
            {
                additionalData = ex.Data;
            }

            if (message.IsEmpty())
            {
                // Defaults to English Key in case Translation is not found.
                message = key;
            }

            return new ErrorResponse(key, message)
            {
                AdditionalData = additionalData
            };
        }
    }
}
