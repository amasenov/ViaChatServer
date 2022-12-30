using System;
using Microsoft.Extensions.Configuration;
using ViaChatServer.BuildingBlocks.Infrastructure.Exceptions;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Extensions
{
    public static class ConfigurationExtensions
    {
        private const string ConnectionStringsSection = "ConnectionStrings";

        public static string GetConnectionStringSetting(this IConfiguration configuration, string settingName, bool required = true)
        {
            if (configuration.IsNull())
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (settingName.IsEmpty())
            {
                throw new ArgumentNullException(nameof(settingName));
            }

            var settingValue = configuration.GetConnectionString(settingName);

            if (settingValue.IsEmpty())
            {
                settingValue = configuration.GetSectionSetting(ConnectionStringsSection, settingName, required);
            }

            return settingValue;
        }

        public static string GetSectionSetting(this IConfiguration configuration, string section, string settingName, bool required = true)
        {
            if (section.IsEmpty())
            {
                throw new ArgumentNullException(nameof(section));
            }

            if (settingName.IsEmpty())
            {
                throw new ArgumentNullException(nameof(settingName));
            }

            var combinedSettingName = $"{section}__{settingName}";
            var settingValue = configuration.GetValue<string>(combinedSettingName);
            if(settingValue.IsEmpty())
            {
                settingValue = configuration.GetValue<string>(combinedSettingName.Replace("__", ":"));
            }
            //var settingValue = configuration[combinedSettingName];

            if (required && settingValue.IsEmpty())
            {
                throw new ConfigurationException(combinedSettingName);
            }

            return settingValue;
        }
    }
}
