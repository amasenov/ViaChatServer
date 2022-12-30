using System.Net;
using System.Net.Http;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Exceptions
{
    /// <summary>
    /// Application specific exception that can be used when the authorization requirements are not met.
    /// </summary>
    public class ForbiddenException : HttpRequestException
    {
        public ForbiddenException(string message = "The access is forbidden for the current user.") : base(message, null, HttpStatusCode.Forbidden)
        {
        }
    }
}
