using Microsoft.Extensions.Configuration;

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
        public string GetDatabaseConnection() => GetDatabaseConnectionString(_configuration);
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
