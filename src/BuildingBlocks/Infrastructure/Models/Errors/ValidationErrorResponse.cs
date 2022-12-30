using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Models
{
    /// <summary>
    /// Generic validation error response which is used when the provided input isn't valid.
    /// </summary>
    public record ValidationErrorResponse : ErrorResponse
    {
        private class CollectionWrapper
        {
            public List<KeyValuePair<string, string>> DetailedErrorMessages { get; internal set; }
        }

        private ValidationErrorResponse(string key, string message) : base(key, message)
        {
        }

        public ValidationErrorResponse(string key, string message, ModelStateDictionary modelStates) : this(key, message)
        {
            if (modelStates.IsNull())
            {
                throw new ArgumentNullException(nameof(modelStates));
            }

            var detailedErrorMessages = new List<KeyValuePair<string, string>>();
            foreach (var modelState in modelStates)
            {
                if (modelState.Value.ValidationState != ModelValidationState.Invalid)
                {
                    continue;
                }

                foreach(var error in modelState.Value.Errors)
                {
                    if (!string.IsNullOrEmpty(error?.ErrorMessage))
                    {
                        detailedErrorMessages.Add(new KeyValuePair<string, string>(modelState.Key, error?.ErrorMessage));
                    }
                }
            }

            base.AdditionalData = new CollectionWrapper() { DetailedErrorMessages = detailedErrorMessages };
        }

        public ValidationErrorResponse(string key, string message, string detailedErrorMessage) : this(key, message, new string[] { detailedErrorMessage })
        {
        }

        public ValidationErrorResponse(string key, string message, IEnumerable<string> detailedErrorMessages) : this(key, message)
        {
            base.AdditionalData = detailedErrorMessages;
        }
    }
}
