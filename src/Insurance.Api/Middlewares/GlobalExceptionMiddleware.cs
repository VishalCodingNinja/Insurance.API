using Insurance.Api.Consts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Insurance.Api.Middlewares
{
    /// <summary>
    /// Middleware to handle global exceptions by mapping exceptions to HTTP status codes and messages.
    /// </summary>
    public class GlobalExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly Dictionary<Type, (HttpStatusCode StatusCode, string Message)> _exceptionMappings;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
            _exceptionMappings = InitializeExceptionMappings();
        }

        /// <summary>
        /// Initializes exception mappings for different exception types.
        /// </summary>
        /// <returns>Dictionary mapping exception types to status codes and messages.</returns>
        private static Dictionary<Type, (HttpStatusCode, string)> InitializeExceptionMappings()
        {
            var mappings = new Dictionary<Type, (HttpStatusCode, string)>
            {
                { typeof(InvalidOperationException), (HttpStatusCode.BadRequest, ApiConstants.ExceptionMessages.InvalidOperation) },
                { typeof(ArgumentNullException), (HttpStatusCode.BadRequest, ApiConstants.ExceptionMessages.NullParameter) },
                { typeof(UnauthorizedAccessException), (HttpStatusCode.Unauthorized, ApiConstants.ExceptionMessages.AccessDenied) },
                { typeof(NotSupportedException), (HttpStatusCode.NotImplemented, ApiConstants.ExceptionMessages.NotSupported) },
                { typeof(KeyNotFoundException), (HttpStatusCode.NotFound, ApiConstants.ExceptionMessages.ResourceNotFound) },
                { typeof(TimeoutException), (HttpStatusCode.RequestTimeout, ApiConstants.ExceptionMessages.Timeout) },
                { typeof(ApplicationException), (HttpStatusCode.BadRequest, ApiConstants.ExceptionMessages.BadRequest) },
                { typeof(NotImplementedException), (HttpStatusCode.NotImplemented, ApiConstants.ExceptionMessages.NotImplemented) }
            };

            // Adding SqlException mapping for different namespaces
            mappings.Add(Type.GetType("System.Data.SqlClient.SqlException"), (HttpStatusCode.ServiceUnavailable, ApiConstants.ExceptionMessages.DatabaseError));
            mappings.Add(Type.GetType("Microsoft.Data.SqlClient.SqlException"), (HttpStatusCode.ServiceUnavailable, ApiConstants.ExceptionMessages.DatabaseError));

            return mappings;
        }

        /// <summary>
        /// Processes exceptions and sets the appropriate status code and response message.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <param name="exception">The exception that was thrown.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = GetStatusCodeAndMessageForException(exception);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsJsonAsync(new { message });
        }

        /// <summary>
        /// Gets the appropriate HTTP status code and message for a given exception.
        /// </summary>
        /// <param name="exception">The exception that occurred.</param>
        /// <returns>Tuple containing the status code and message.</returns>
        private (HttpStatusCode StatusCode, string Message) GetStatusCodeAndMessageForException(Exception exception)
        {
            if (_exceptionMappings.TryGetValue(exception.GetType(), out var mapping))
            {
                return mapping;
            }

            _logger.LogError(exception, "Unhandled exception.");
            return (HttpStatusCode.InternalServerError, ApiConstants.ExceptionMessages.DefaultErrorMessage);
        }
    }
}
