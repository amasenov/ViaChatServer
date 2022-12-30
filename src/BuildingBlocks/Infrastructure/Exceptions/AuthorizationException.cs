using System.Net;
using System.Net.Http;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Exceptions
{
    /// <summary>
    /// Application specific exception that can be used when the authorization requirements are not met.
    /// </summary>
    public class AuthorizationException : HttpRequestException
    {
        public AuthorizationException(string message = "The current user is not authorized.") : base(message, null, HttpStatusCode.Unauthorized)
        {
        }
    }
}
