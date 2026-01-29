using Application.Shared.Helpers.Exceptions;
using Application.Shared.Helpers.Responses;
using System.Text.Json;

namespace API.MiddleWares;

public sealed class GlobalExceptionMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception ex)
    {
        // Default
        var status = StatusCodes.Status500InternalServerError;
        var message = "Unexpected server error.";
        List<string>? errors = null;

        // Custom API exceptions (preferred)
        if (ex is ApiException apiEx)
        {
            status = apiEx.StatusCode;
            message = apiEx.Message;
            errors = apiEx.Errors?.ToList();
        }
        // FluentValidation exception 
        else if (ex.GetType().Name.Contains("ValidationException", StringComparison.OrdinalIgnoreCase))
        {
            status = StatusCodes.Status400BadRequest;
            message = "Validation failed.";
        }
        // Argument exceptions
        else if (ex is ArgumentException argEx)
        {
            status = StatusCodes.Status400BadRequest;
            message = argEx.Message;
        }

        // Log
        _logger.LogError(ex,
            "Unhandled exception. TraceId: {TraceId} Path: {Path}",
            context.TraceIdentifier,
            context.Request.Path);

        // Response format: BaseResponse
        var response = BaseResponse.Fail(
            message: message,
            statusCode: status,
            errors: errors
        );

        if (_env.IsDevelopment())
        {

        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = status;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
