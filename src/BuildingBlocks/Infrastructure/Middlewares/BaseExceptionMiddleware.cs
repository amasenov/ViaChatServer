using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Exceptions;
using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;
using ViaChatServer.BuildingBlocks.Infrastructure.Models;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Middlewares
{
    /// <summary>
    /// Middleware that processes the unhandled exceptions.
    /// </summary>
    public record BaseExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BaseExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public BaseExceptionMiddleware(RequestDelegate next, ILogger<BaseExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environment = env ?? throw new ArgumentNullException(nameof(env));
        }

        public virtual async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning(ex, "The response has already started, the http status code middleware will not be executed.");
                    throw;
                }

                await UpdateResponseAccordingToExceptionAsync(ex, context.Response, _logger, _environment);
            }
        }

        public static async Task UpdateResponseAccordingToExceptionAsync(Exception exception, HttpResponse response, ILogger<BaseExceptionMiddleware> logger, IHostEnvironment env, HttpStatusCode? statusCode = null, ErrorResponse errorResponse = null)
        {
            var result = GetErrorReponseAndStatusCode(exception, logger, env);
            statusCode ??= result.statusCode;
            errorResponse ??= result.errorResponse;

            response.Clear();
            response.AddApplicationErrorHeaders(exception.Message);
            response.StatusCode = (int)statusCode;

            if (errorResponse != null)
            {
                response.ContentType = MediaTypeNames.Application.Json;
                await response.WriteAsync(errorResponse.ToJson());
            }
        }

        private static (HttpStatusCode statusCode, ErrorResponse errorResponse) GetErrorReponseAndStatusCode(Exception exception, ILogger<BaseExceptionMiddleware> logger, IHostEnvironment env)
        {
            if (exception is AuthorizationException || (exception.InnerException.HasValue() && exception.InnerException is AuthorizationException))
            {
                Exception authorizationException = exception is AuthorizationException ? exception : exception.InnerException;
                ErrorResponse response = GetDevelopmentError(HttpStatusCode.Unauthorized, authorizationException, env);
                return (HttpStatusCode.Unauthorized, response);
            }
            else if (exception is ForbiddenException || (exception.InnerException.HasValue() && exception.InnerException is ForbiddenException))
            {
                Exception forbiddenException = exception is ForbiddenException ? exception : exception.InnerException;
                ErrorResponse response = GetDevelopmentError(HttpStatusCode.Forbidden, forbiddenException, env);
                return (HttpStatusCode.Forbidden, response);
            }
            else if (exception is EntityNotFoundException || (exception.InnerException.HasValue() && exception.InnerException is EntityNotFoundException))
            {
                Exception entityNotFoundException = exception is EntityNotFoundException ? exception : exception.InnerException;
                ErrorResponse response = GetDevelopmentError(HttpStatusCode.NotFound, entityNotFoundException, env);
                return (HttpStatusCode.NotFound, response);
            }
            else if (exception is ValidationException || (exception.InnerException.HasValue() && exception.InnerException is ValidationException))
            {
                Exception validationException = exception is ValidationException ? exception : exception.InnerException;
                return (HttpStatusCode.BadRequest, (ValidationException)validationException);
            }
            else
            {
                ErrorResponse response = GetDevelopmentError(HttpStatusCode.InternalServerError, exception, env);

                LogUnexpectedException(exception, logger);
                return (HttpStatusCode.InternalServerError, response);
            }
        }

        private static ErrorResponse GetDevelopmentError(HttpStatusCode code, Exception exception, IHostEnvironment env)
        {
            return env.IsDevelopment() ? new ErrorResponse(code.ToString(), exception.Message) 
            { 
                AdditionalData = new 
                { 
                    Message = exception?.InnerException?.Message,
                    InnerMessage = exception?.InnerException?.InnerException?.Message
                } 
            } : null;
        }

        private static void LogUnexpectedException(Exception ex, ILogger<BaseExceptionMiddleware> logger)
        {
            try
            {
                logger.LogError(ex, "Unhandled exception occurred: {@ErrorMessage} | {@InnerMessage}", ex.Message, ex.InnerException?.Message);
            }
            catch
            {
                // If writing to the log fails, then just respond the error.
            }
        }
    }
}
