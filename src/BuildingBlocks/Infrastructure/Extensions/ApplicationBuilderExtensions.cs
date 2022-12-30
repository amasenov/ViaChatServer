using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IApiVersionDescriptionProvider provider, string pathBase, string clientId)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(ConfigureSwagger);

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI((options) => ConfigureSwaggerUI(app, options, provider, pathBase, clientId));

            return app;
        }

        public static IApplicationBuilder UseUpdateDatabase<TContext>(this IApplicationBuilder app) where TContext : DbContext
        {
            // Create a new scope to retrieve scoped services
            using IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            // Get the DbContext instance
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

            //Do the migration by blocking the async call
            dbContext.Database.Migrate();

            return app;
        }

        public static IApplicationBuilder UseCookiePolicyOptions(this IApplicationBuilder app)
        {
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                CheckConsentNeeded = context => true,
                MinimumSameSitePolicy = SameSiteMode.None,
                Secure = CookieSecurePolicy.Always
            });

            return app;
        }

        #region Private Methods
        private static void ConfigureSwagger(SwaggerOptions options)
        {

        }

        private static void ConfigureSwaggerUI(IApplicationBuilder app, SwaggerUIOptions options, IApiVersionDescriptionProvider provider, string pathBase, string clientId)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.DocExpansion(DocExpansion.None);
                options.EnableFilter();
                options.DefaultModelRendering(ModelRendering.Model);
                options.DefaultModelsExpandDepth(-1);

                options.SwaggerEndpoint($"{(pathBase.HasValue() ? pathBase : string.Empty)}/swagger/{description.GroupName}/swagger.json", $"My API {description.GroupName.ToUpperInvariant()}");

                app.UseReDoc(o =>
                {
                    o.DocumentTitle = "Chat API - Documentation";
                    o.SpecUrl = $"/swagger/{description.GroupName}/swagger.json";
                });

                options.OAuthClientId(clientId);
                options.OAuthAppName("Chat API - Swagger");
                options.OAuthUsePkce();
                //options.OAuthClientSecret("");
                //options.OAuth2RedirectUrl("")
            }
        }
        #endregion
    }
}
