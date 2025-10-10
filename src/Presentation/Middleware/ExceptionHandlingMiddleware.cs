using Domain.Exceptions;
using Domain.ValueObjects;
using System.Text.Json;

namespace Presentation.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            //_logger.LogError(ex, "An unhandled exception occurred.");
            //context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            //context.Response.ContentType = "application/json";
            //var response = new
            //{
            //    Message = "An unexpected error occurred. Please try again later."
            //};
            //await context.Response.WriteAsJsonAsync(response);

            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            PromoCodeNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            PromoCodeAlreadyExistsException => (StatusCodes.Status409Conflict, exception.Message),
            PromoCodeValidationException => (StatusCodes.Status400BadRequest, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.")

        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new
        {
            error = message
        };
        return context.Response.WriteAsJsonAsync(JsonSerializer.Serialize(response));
    }
}
