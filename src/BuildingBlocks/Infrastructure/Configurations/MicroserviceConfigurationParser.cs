using Microsoft.Extensions.Configuration;

using System;

using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Configurations
{
    public class MicroserviceConfigurationParser
    {
        private readonly IConfiguration _configuration;

        public MicroserviceConfigurationParser(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #region Application settings
        public ApplicationSettings GetApplicationSettings()
        {
            const string sectionName = "AppSettings";

            Uri authorityUrl = _configuration.GetUriSectionSetting(sectionName, "AuthorityUrl");

            string databaseConnectionString = GetDatabaseConnectionString(_configuration);

            return ApplicationSettings.GetInstance(databaseConnectionString, authorityUrl);
        }
        #endregion

        #region Private methods
        private static string GetDatabaseConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionStringSetting("ChatDatabaseConnectionString", false);

            return @connectionString;
        }
        #endregion
    }
}
