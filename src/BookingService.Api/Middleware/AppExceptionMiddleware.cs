using BookingService.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace BookingService.Api.Middleware;

public class AppExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AppExceptionMiddleware> _logger;

    public AppExceptionMiddleware(RequestDelegate next, ILogger<AppExceptionMiddleware> logger)
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
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "AppException: {Message}", ex.Message);
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = ex.StatusCode;
            var body = JsonSerializer.Serialize(new { error = ex.Message });
            await context.Response.WriteAsync(body);
        }
    }
}
