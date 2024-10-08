﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Insurance.Api.Middlewares
{
    /// <summary>
    /// Middleware to handle global exceptions for the application.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the request pipeline.</param>
        /// <param name="logger">The logger instance used for logging exceptions.</param>
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware to handle requests and catch any unhandled exceptions.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles exceptions by setting the appropriate status code and response message.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <param name="exception">The exception that was thrown.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError; // Default to 500 if unexpected
            var result = new { message = "An unexpected error occurred. Please try again later." };


            if (exception is InvalidOperationException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                result = new { message = "Invalid operation." };
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsJsonAsync(result);
        }
    }
}
