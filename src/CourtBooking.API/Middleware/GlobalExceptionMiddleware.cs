using FluentValidation;
using System.Net;
using System.Text.Json;

namespace CourtBooking.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
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
        catch (ValidationException ex)
        {
            // Errores de FluentValidation — 400 Bad Request
            _logger.LogWarning("Validation error: {Errors}", ex.Errors);
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            // Cualquier otra excepción — 500 Internal Server Error
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleValidationExceptionAsync(
        HttpContext context,
        ValidationException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        var response = new
        {
            type = "ValidationError",
            title = "One or more validation errors occurred.",
            status = 400,
            errors
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
    }

    private static Task HandleExceptionAsync(
        HttpContext context,
        Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            type = "ServerError",
            title = "An unexpected error occurred.",
            status = 500,
            detail = ex.Message
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
    }
}