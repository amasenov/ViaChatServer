using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Extensions
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Retrieve the raw body as a string from the Request.Body stream
        /// </summary>
        /// <param name="request">Request instance to apply to</param>
        /// <returns></returns>
        public static async Task<string> GetBodyAsync(this HttpRequest request, bool leaveOpen = true)
        {
            string content = string.Empty;
            if(request.Body.CanSeek)
            {
                // If request is POST, PUT, PATCH, then EnableBuffering() should be enabled already through EnableRequestBodyBufferingMiddleware
                request.Body.Position = 0;

                using StreamReader reader = new(request.Body, Encoding.UTF8, leaveOpen: leaveOpen);

                content = await reader.ReadToEndAsync().ConfigureAwait(false);

                request.Body.Position = 0;
            }

            return content;
        }

        public static string GetRawHeaders(this HttpRequest request) => string.Join(", ", request.Headers.Select(x => $"{{{x.Key}: {string.Join(", ", x.Value)}}}"));
        public static string GetRawQueries(this HttpRequest request) => string.Join(", ", request.Query.Select(x => $"{{{x.Key}: {string.Join(", ", x.Value)}}}"));

        public static HttpResponseMessage CreateResponse(this HttpRequest request, HttpStatusCode statusCode = HttpStatusCode.OK) => new(statusCode);

        public static string GetAccessToken(this HttpRequest request) => request.Headers[HeaderNames.Authorization];
        public static Claim GetClaim(this HttpRequest request, string type) => request.HttpContext.User.FindFirst(type);
        public static bool IsInRole(this HttpRequest request, string role) => request.HttpContext.User.IsInRole(role);

        public static NameValueCollection GetQueryParameters(this HttpRequest request) => request.QueryString.HasValue ? HttpUtility.ParseQueryString(request.QueryString.Value) : new();
        public static string GetQueryParameter(this HttpRequest request, string key) => request.GetQueryParameters().Get(key);
    }
}
