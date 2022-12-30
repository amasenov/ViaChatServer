using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using System;
using System.IdentityModel.Tokens.Jwt;

using ViaChatServer.BuildingBlocks.Infrastructure.Constants;
using ViaChatServer.BuildingBlocks.Infrastructure.LogConfigurations;
using ViaChatServer.BuildingBlocks.Infrastructure.Models;
using ViaChatServer.BuildingBlocks.Infrastructure.Resources;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuth(this IServiceCollection services, string authority, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Audience = SupportedScopes.ChatApi;
                    options.Authority = authority;
                    options.RequireHttpsMetadata = true;

                    if (bool.TryParse(configuration["VALIDATE_JWT"], out bool validateJwt) && !validateJwt)
                    {
                        options.TokenValidationParameters.SignatureValidator = delegate (string t, TokenValidationParameters p)
                        {
                            return new JwtSecurityToken(t);
                        };
                    }

                    options.TokenValidationParameters.ValidateLifetime = !bool.TryParse(configuration["VALIDATE_JWT_LIFETIME"], out bool validateJwtLifetime) || validateJwtLifetime;
                });

            return services;
        }
        public static IServiceCollection AddApiBehaviorOptions(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    return new BadRequestObjectResult(new ValidationErrorResponse(ApiDefinitions.GenericValidationKey, ModelValidation.GenericModelValidation, context.ModelState));
                };
            });

            return services;
        }

        public static IServiceCollection AddHstsService(this IServiceCollection services, bool isProduction)
        {
            if (isProduction)
            {
                services.AddHsts(options =>
                {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromDays(365);
                    options.ExcludedHosts.Add("ridewithvia.com");
                    options.ExcludedHosts.Add("www.ridewithvia.com");
                });
            }

            return services;
        }

        public static IServiceCollection AddForwardHeader(this IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.RequireHeaderSymmetry = false;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            return services;
        }
        public static IServiceCollection AddVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            return services;
        }

        public static IServiceCollection AddTelementry(this IServiceCollection services, IConfiguration configuration)
        {
            ApplicationInsightsServiceOptions options = new()
            {
                EnableAdaptiveSampling = true,
                EnablePerformanceCounterCollectionModule = true,
                EnableDependencyTrackingTelemetryModule = false,
                EnableEventCounterCollectionModule = true
            };
            // The following line enables Application Insights telemetry collection.
            services.AddApplicationInsightsTelemetry(options);
            services.AddApplicationInsightsTelemetryProcessor<OpertationFilter>();

            return services;
        }
    }
}
