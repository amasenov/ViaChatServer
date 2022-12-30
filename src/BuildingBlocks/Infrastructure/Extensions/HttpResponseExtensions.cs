using Microsoft.AspNetCore.Http;

using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Extensions
{
    /// <summary>
    /// Http response header extension methods
    /// </summary>
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Retrieve the raw body as a string from the Request.Body stream
        /// </summary>
        /// <param name="request">Request instance to apply to</param>
        /// <param name="encoding">Optional - Encoding, defaults to UTF8</param>
        /// <returns></returns>
        public static async Task<string> GetRawBodyStringAsync(this HttpResponse request, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;

            using StreamReader reader = new(request.Body, encoding);
            var content = await reader.ReadToEndAsync();

            return content;
        }
        public static async Task<TBody> GetBodyAsync<TBody>(this HttpResponse request, Encoding encoding = null) where TBody : new()
        {
            var content = await request.GetRawBodyStringAsync(encoding);

            return content.FromJson<TBody>();
        }

        /// <summary>
        /// Adds the application error headers.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="message">The message.</param>
        public static void AddApplicationErrorHeaders(this HttpResponse response, string message)
        {
            try
            {
                AddOrReplaceHeader(response.Headers, "Application-Error", message);
                AddOrReplaceHeader(response.Headers, "access-control-expose-headers", "Application-Error");
                AddOrReplaceHeader(response.Headers, "Access-Control-Allow-Origin", "*");
            }
            catch
            {
            }

        }

        /// <summary>
        /// Adds the or replace the specified header.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private static void AddOrReplaceHeader(IHeaderDictionary headers, string key, string value)
        {
            if (headers.ContainsKey(key))
            {
                headers[key] = value;
            }
            else
            {
                headers.Add(key, value);
            }
        }
    }
}
