using Autofac;

using Chat.API.Exceptions;
using Chat.API.Hubs;
using Chat.API.Models;
using Chat.Application;
using Chat.Persistence;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;

using System.Collections.Generic;
using System.Reflection;

using ViaChatServer.BuildingBlocks.Infrastructure.Configurations;
using ViaChatServer.BuildingBlocks.Infrastructure.Constants;
using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;
using ViaChatServer.BuildingBlocks.Infrastructure.LogConfigurations;
using ViaChatServer.BuildingBlocks.Infrastructure.Middlewares;
using ViaChatServer.BuildingBlocks.Infrastructure.Serializing;
using ViaChatServer.BuildingBlocks.Infrastructure.Swagger;

namespace Chat.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Environment = env;
            Configuration = configuration;
        }

        public IHostEnvironment Environment { get; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            MicroserviceConfigurationParser configurationParser = new(Configuration);

            var appSettings = configurationParser.GetApplicationSettings();

            // The following line enables Application Insights telemetry collection.
            services.AddTelementry(Configuration);

            services.AddAuth(appSettings.AuthorityUrl.AbsoluteUri, Configuration);

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllers()
                .AddJsonOptions(options => SerializingSettings.JsonOptions(options));

            services.AddSignalR();            // Add this service too

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddHstsService(Environment.IsProduction());

            services.AddApiBehaviorOptions();

            services.AddVersioning();
            services.AddForwardHeader();

            services.AddSwaggerGen();
            services.ConfigureOptions(new ConfigureSwaggerOptions(services, Assembly.GetExecutingAssembly().GetName().Name, appSettings.AuthorityUrl));

            services.AddSingleton(appSettings);
            services.AddSingleton<IDictionary<string, UserConnection>>(opts => new Dictionary<string, UserConnection>());

            services.AddChatPersistence(appSettings.DatabaseConnectionString);
            services.AddRepositories();

            services.AddServices();

            //// Add services to the collection
            services.AddOptions();
        }

        #region Autofac
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Required for Autofac configuration
            // Add things to the Autofac ContainerBuilder.
        }
        #endregion

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, ILoggerFactory loggerFactory, IApiVersionDescriptionProvider provider)
        {
            //apply db migrations automatically on start
            app.UseUpdateDatabase<ChatDbContext>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else if (env.IsProduction())
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseForwardedHeaders();
            app.UseHttpsRedirection();
            app.UseWhen(
                ctx => ctx.Request.Method.IsEqual(HttpMethods.Post) || ctx.Request.Method.IsEqual(HttpMethods.Put) || ctx.Request.Method.IsEqual(HttpMethods.Patch),
                ab => ab.UseMiddleware<EnableRequestBodyBufferingMiddleware>()
            );
            app.UseRequestLocalization(LocalizationValues.LocalizationOptions());
            app.UseSerilogRequestLogging(SerilogSettings.RequestLoggingOptions);
            app.UseMiddleware<ExceptionMiddleware>();

            var pathBase = Configuration["PATH_BASE"];
            app.UseSwaggerDocumentation(provider, pathBase, Configuration["SwaggerClientId"]);

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHub<ChatHub>("/chat");     // path will look like this https://localhost:44382/chat 
            });
        }
    }
}
