using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

using System.Collections.Generic;
using System.Globalization;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Constants
{
    /// <summary>
    /// Localization specific values.
    /// </summary>
    public static class LocalizationValues
    {
        private const string englishLocaleCode = "en-us";

        public static readonly CultureInfo EnglishCulture = CultureInfo.GetCultureInfo(englishLocaleCode);

        public static readonly RegionInfo EnglishRegion = new(EnglishCulture.LCID);

        public static List<CultureInfo> SupportedCultures() => new()
        {
            EnglishCulture
        };

        public static RequestLocalizationOptions LocalizationOptions() => new()
        {
            DefaultRequestCulture = new RequestCulture(EnglishCulture),
            SupportedCultures = SupportedCultures(),
            SupportedUICultures = SupportedCultures(),
            FallBackToParentCultures = true
        };
    }
}
