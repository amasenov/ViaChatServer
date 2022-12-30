using System;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;

namespace ViaChatServer.BuildingBlocks.Infrastructure.LogConfigurations
{
    public static class SerilogSettings
    {
        public static void RequestLoggingOptions(RequestLoggingOptions options)
        {
            // Default: HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms
            // ApplicationName:     Name of the Application
            // ClientIP: 	        IP Address of the client the request originated from
            // CorrelationId:       ID that can be used to trace request across multiple application boundaries
            // Elapsed:             Time in milliseconds an operation takes
            // EventType:           Hash of the message template to determine the message type
            // MachineName:         Name of the machine which application is running
            // Outcome:             The result of the operation
            // RequestMethod:       HTTP request method name
            // RequestPath:         Http request path
            // SourceContext:       Name of component/class which the log originated
            // StatusCode:          Http status code
            // UserAgent:           Http User Agent
            // Version:             Version of the application
            // Customize the message template
            options.MessageTemplate = "Handled {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms | UserAgent {UserAgent}";

#if DEBUG
            // Emit debug-level events instead of the defaults
            //options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;
#endif

            // Attach additional properties to the request completion event
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                bool hasBody = httpContext.Request.Method.IsEqual(HttpMethods.Post) || httpContext.Request.Method.IsEqual(HttpMethods.Put) || httpContext.Request.Method.IsEqual(HttpMethods.Patch);
                string requestBody = hasBody ? httpContext.Request.GetBodyAsync(false).GetAwaiter().GetResult() : string.Empty;
                
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("RequestBody", requestBody);
                diagnosticContext.Set("RequestHeaders", httpContext.Request.GetRawHeaders());
                diagnosticContext.Set("QueryString", httpContext.Request.QueryString.Value);
                //diagnosticContext.Set("Route", httpContext.Request.RouteValues);
            };
        }

        // NOTE: According to fixes proposed here: https://github.com/serilog/serilog-sinks-applicationinsights/issues/121
        public static void ConfigureLogger(HostBuilderContext context, IServiceProvider services, LoggerConfiguration configuration) =>
            configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers())
            .Destructure.ToMaximumDepth(maximumDestructuringDepth: 4)
            .Destructure.ToMaximumCollectionCount(maximumCollectionCount: 10)
            .WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces);
    }
}
