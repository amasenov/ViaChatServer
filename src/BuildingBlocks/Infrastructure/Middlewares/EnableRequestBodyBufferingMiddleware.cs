using Microsoft.AspNetCore.Http;

using System.Threading.Tasks;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Middlewares
{
    public record EnableRequestBodyBufferingMiddleware
    {
        private readonly RequestDelegate _next;

        public EnableRequestBodyBufferingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            await _next(context);
        }
    }
}
