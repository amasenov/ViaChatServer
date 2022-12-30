namespace ViaChatServer.BuildingBlocks.Infrastructure.LogConfigurations
{
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.Extensibility.Implementation;

    using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;

    public record OpertationFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;
        private readonly bool _isLoggingEnabled;

        public OpertationFilter(ITelemetryProcessor next)
        {
            _next = next;

            // Logging should be enabled only when in live environment and not while debugging
#if DEBUG
            _isLoggingEnabled = false;
#else
            _isLoggingEnabled = true;
#endif
        }

        public void Process(ITelemetry item)
        {
            if (OkToSend(item))
            {
                _next.Process(item);
            }

            return;
        }

        private bool OkToSend(ITelemetry item)
        {
            if (_isLoggingEnabled)
            {
                if (item is MetricTelemetry metric)
                {
                    _ = metric;

                    return true;
                }
                else if (item is TraceTelemetry trace)
                {
                    _ = trace;

                    return trace.Properties.TryGetValue("RequestPath", out string requestPath) && (requestPath.IsContaining("/api/"));
                }
                else if (item is RequestTelemetry request)
                {
                    _ = request;

                    return request.Url.HasValue() && (request.Url.AbsolutePath.IsContaining("/api/"));
                }
                else if (item is ExceptionTelemetry exception)
                {
                    _ = exception;

                    return true;
                }
                else if (item is OperationTelemetry operation)
                {
                    _ = operation;
                    return operation.Success != true;
                }
            }

            return false;
        }
    }
}
