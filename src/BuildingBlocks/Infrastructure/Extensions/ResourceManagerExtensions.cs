using System;
using System.Globalization;
using System.Resources;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Extensions
{
    /// <summary>
    /// Resource manager extension methods.
    /// </summary>
    public static class ResourceManagerExtensionsMethods
    {
        /// <summary>
        /// Gets the translation with specified key.
        /// </summary>
        /// <param name="resourceManager">The resource manager.</param>
        /// <param name="key">The key.</param>
        /// <returns>The translation with specified key.</returns>
        public static string GetTranslation(this ResourceManager resourceManager, string key) => GetTranslationOfCurrentCulture(resourceManager, key);
        public static string GetTranslation(this ResourceManager resourceManager, Enum key) => GetTranslationOfCurrentCulture(resourceManager, key.ToString());

        /// <summary>
        /// Gets the translation of the current culture.
        /// </summary>
        /// <param name="resourceManager">The resource manager.</param>
        /// <param name="key">The key.</param>
        /// <returns>The translation</returns>
        private static string GetTranslationOfCurrentCulture(ResourceManager resourceManager, string key)
        {
            try
            {
                return resourceManager?.GetString(key, CultureInfo.CurrentUICulture) ?? throw new ArgumentNullException(nameof(resourceManager));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Missing translation for validation error message for {key}", ex);
            }
        }
    }
}
