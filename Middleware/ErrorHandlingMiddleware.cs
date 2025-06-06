using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace BookApi.Middleware
{
    /// <summary>
    /// Global error handling middleware
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred");

            var statusCode = GetStatusCode(exception);
            var response = new
            {
                StatusCode = statusCode,
                Message = GetErrorMessage(exception),
                Details = _environment.IsDevelopment() ? exception.StackTrace : null,
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }

        private static int GetStatusCode(Exception exception)
        {
            // Map common exceptions to appropriate status codes
            return exception switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                ArgumentException or ArgumentNullException or InvalidOperationException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                NotImplementedException => (int)HttpStatusCode.NotImplemented,
                _ => (int)HttpStatusCode.InternalServerError
            };
        }

        private static string GetErrorMessage(Exception exception)
        {
            // Return user-friendly error messages for common exceptions
            return exception switch
            {
                KeyNotFoundException => "The requested resource was not found.",
                ArgumentException or ArgumentNullException => "Invalid arguments provided.",
                InvalidOperationException => "The requested operation is invalid.",
                UnauthorizedAccessException => "You are not authorized to access this resource.",
                NotImplementedException => "This functionality is not implemented yet.",
                _ => "An unexpected error occurred. Please try again later."
            };
        }
    }

    // Extension method for registering the middleware
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
