using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;

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

        public static void SetCurrentCulture(string cultureCode = null)
        {
            var cultureInfo = GetSupportedCulture(cultureCode);

            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
        }

        public static CultureInfo GetCurrentCulture() => CultureInfo.CurrentCulture.Name.HasValue() && !CultureInfo.CurrentCulture.IsNeutralCulture ? CultureInfo.CurrentCulture : EnglishCulture;
        public static CultureInfo GetSupportedCulture(string cultureCode = null)
        {
            CultureInfo cultureInfo;
            try
            {
                var validCulture = CultureInfo.GetCultureInfo(cultureCode);
                var supportedCulture = SupportedCultures()
                                        .Where(x => validCulture.IsNeutralCulture ?
                                                    (x.Parent.LCID == validCulture.LCID) :
                                                    ((validCulture.LCID == x.LCID) || (validCulture.Parent.LCID == x.Parent.LCID)))
                                        .FirstOrDefault();
                cultureInfo = supportedCulture ?? EnglishCulture;
            }
            catch
            {
                cultureInfo = EnglishCulture;
            }

            return cultureInfo;
        }
    }
}
