using Domain.Exceptions;

namespace Presentation.Middlewares
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandler> _logger;

        public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var (statusCode, message) = ex switch
            {
                BusinessRuleException => (StatusCodes.Status400BadRequest, ex.Message),
                ForbiddenException => (StatusCodes.Status403Forbidden, ex.Message),
                NotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                UnauthorizedException => (StatusCodes.Status401Unauthorized, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error has occured")
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
                _logger.LogError(ex, "Unhandled exception at {RequestPath}", context.Request.Path);
            else
                _logger.LogWarning("{ExceptionType} at {Path}, {Message}", ex.GetType().Name, context.Request.Path, ex.Message);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new { message = message };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
