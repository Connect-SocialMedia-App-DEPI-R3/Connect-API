namespace Api.Middleware;

/// <summary>
/// Catches all unhandled exceptions in the application
/// Returns consistent error responses and logs errors
/// Better than default error page in production
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, error) = exception switch
        {
            UnauthorizedAccessException => (403, "You don't have permission to access this resource"),
            KeyNotFoundException => (404, exception.Message),
            ArgumentException => (400, exception.Message),
            InvalidOperationException => (400, exception.Message),
            _ => (500, "An internal server error occurred")
        };

        context.Response.StatusCode = statusCode;

        var response = new
        {
            statusCode,
            message = exception.Message,
            error,
            // Only include detailed error in development
            stackTrace = _env.IsDevelopment() ? exception.StackTrace : null
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
