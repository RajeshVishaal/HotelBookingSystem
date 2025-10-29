using System.Net;
using System.Text.Json;
using Common.Dto;
using Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BaseException ex)
        {
            _logger.LogWarning(ex, "Application exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex.StatusCode, ex.Message, ex.ErrorDetails);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Argument validation error: {Message}", ex.Message);
            await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message,
                new { Type = "ArgumentException", Parameter = ex.ParamName });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt");
            await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, "Unauthorized access.",
                new { Type = "UnauthorizedAccessException" });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "External service communication error");
            await HandleExceptionAsync(context, HttpStatusCode.ServiceUnavailable,
                "A dependent service is currently unavailable. Please try again later.",
                new { Type = "HttpRequestException" });
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Request timeout");
            await HandleExceptionAsync(context, HttpStatusCode.RequestTimeout,
                "The request timed out. Please try again.",
                new { Type = "TaskCanceledException" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError,
                "An unexpected error occurred. Please contact support if the problem persists.",
                new { Type = ex.GetType().Name, StackTrace = GetSafeStackTrace(ex) });
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string message,
        object? errorDetails = null)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = ApiResponse<object>.Fail(message, errorDetails);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }

    private static string? GetSafeStackTrace(Exception ex)
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? ex.StackTrace
            : null;
    }
}