using System;
using System.Reflection;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Constants
{
    /// <summary>
    /// Constant application definitions.
    /// </summary>
    public static class ApiDefinitions
    {
        /// <summary>
        /// The generic validation error key
        /// </summary>
        public const string GenericValidationKey = "ModelValidation";

        /// <summary>
        /// The user agent key
        /// </summary>
        public const string UserAgentKey = "User-Agent";

        /// <summary>
        /// Gets the latest API version indication.
        /// </summary>
        public static string LatestApiVersion(Assembly assembly) => $"v{assembly.GetName().Version.Major}";
    }
}

