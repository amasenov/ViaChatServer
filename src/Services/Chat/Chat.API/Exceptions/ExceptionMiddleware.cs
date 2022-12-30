using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ViaChatServer.BuildingBlocks.Infrastructure.Middlewares;

namespace Chat.API.Exceptions
{
    /// <summary>
    /// Middleware that processes the unhandled exceptions.
    /// </summary>
    public record ExceptionMiddleware : BaseExceptionMiddleware
    {
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env) : base(next, logger, env)
        {
        }
    }
}
