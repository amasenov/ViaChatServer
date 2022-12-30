using System.Net;
using System.Net.Http;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Exceptions
{
    /// <summary>
    /// Application specific exception that can be used if an entity was not found.
    /// </summary>
    public class EntityNotFoundException : HttpRequestException
    {
        public EntityNotFoundException(string message = "The requested resource can not be found.") : base(message, null, HttpStatusCode.NotFound)
        {
        }
    }
}
