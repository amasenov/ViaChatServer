using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using System.Collections.Generic;
using System.Linq;
using System.Net;

using ViaChatServer.BuildingBlocks.Infrastructure.Constants;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Swagger
{
    public record AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check for authorize attribute
            var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                               context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            if (hasAuthorize)
            {
                operation.Responses.TryAdd(((int)HttpStatusCode.Unauthorized).ToString(), new OpenApiResponse { Description = nameof(HttpStatusCode.Unauthorized) });
                operation.Responses.TryAdd(((int)HttpStatusCode.Forbidden).ToString(), new OpenApiResponse { Description = nameof(HttpStatusCode.Forbidden) });

                var oAuthScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [ oAuthScheme ] = new [] { SupportedScopes.ChatApi }
                    }
                };
            }
        }
    }
}
