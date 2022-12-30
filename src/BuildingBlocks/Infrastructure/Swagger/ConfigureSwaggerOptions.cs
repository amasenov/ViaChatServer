using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using System;
using System.Collections.Generic;
using System.IO;

using ViaChatServer.BuildingBlocks.Infrastructure.Constants;
using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Swagger
{
    public record ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly string _executingAssembly;

        public ConfigureSwaggerOptions(IServiceCollection services, string executingAssembly)
        {
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
            _provider = provider;
            _executingAssembly = executingAssembly;
        }

        public void Configure(string name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        public void Configure(SwaggerGenOptions options)
        {
            options.DocInclusionPredicate((docName, apiDescription) =>
            {
                if (apiDescription.HttpMethod.IsNull())
                {
                    return false;
                }
                return true;
            });

            options.OrderActionsBy(api => api.RelativePath);

            options.DescribeAllParametersInCamelCase();

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{_executingAssembly}.xml";
            var xmlPathApi = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPathApi))
            {
                options.IncludeXmlComments(xmlPathApi);
            }

            var xmlFileApplication = xmlFile.Replace("API", "Application");
            var xmlPathApplication = Path.Combine(AppContext.BaseDirectory, xmlFileApplication);
            if (File.Exists(xmlPathApplication))
            {
                options.IncludeXmlComments(xmlPathApplication);
            }

            options.EnableAnnotations();

            // add swagger document for every API version discovered
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
            }

            options.OperationFilter<AcceptLanguageOperationFilter>();
        }

        private static OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
        {
            OpenApiInfo info = new()
            {
                Title = "Chat API",
                Version = description.ApiVersion.ToString(),
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                  {"x-logo", new OpenApiObject
                    {
                       {"url", new OpenApiString("https://skpservices.blob.core.windows.net/images-temp/topology_logo_1000.png")},
                       { "altText", new OpenApiString("Chat")}
                    }
                  }
                }
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
