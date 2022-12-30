using System.Net;
using System.Net.Http;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Exceptions
{
    /// <summary>
    /// Exception that can be used when configuration options are not valid.
    /// </summary>
    /// <seealso cref="HttpRequestException" />
    public class ConfigurationException : HttpRequestException
    {
        public ConfigurationException(string configurationOption) : base($"Configuration is not valid: {configurationOption}", null, HttpStatusCode.InternalServerError)
        {
        }
    }
}
