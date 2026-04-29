using System.Net;
using System.Text.Json;
using TaskCraft.API.Models;
using TaskCraft.Core.Exceptions;

namespace TaskCraft.API.Middleware;

/// <summary>
/// Middleware for handling exceptions globally and returning standardized error responses
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
        var errorResponse = CreateErrorResponse(context, exception);

        // Log the exception with appropriate level
        LogException(exception, errorResponse);

        // Set response headers
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorResponse.StatusCode;

        // Serialize and write response
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        var json = JsonSerializer.Serialize(errorResponse, options);
        await context.Response.WriteAsync(json);
    }

    private ErrorResponse CreateErrorResponse(HttpContext context, Exception exception)
    {
        var errorResponse = new ErrorResponse
        {
            Path = context.Request.Path,
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case BaseCustomException customException:
                errorResponse.StatusCode = customException.StatusCode;
                errorResponse.ErrorCode = customException.ErrorCode;
                errorResponse.Message = customException.Message;
                
                if (customException is ValidationException validationException)
                {
                    errorResponse.Errors = validationException.Errors;
                }
                break;

            case ArgumentNullException argumentNullException:
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.ErrorCode = "NULL_ARGUMENT";
                errorResponse.Message = argumentNullException.Message;
                break;

            case ArgumentException argumentException:
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.ErrorCode = "INVALID_ARGUMENT";
                errorResponse.Message = argumentException.Message;
                break;

            case UnauthorizedAccessException:
                errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.ErrorCode = "UNAUTHORIZED_ACCESS";
                errorResponse.Message = "You are not authorized to perform this operation.";
                break;

            case InvalidOperationException invalidOperationException:
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.ErrorCode = "INVALID_OPERATION";
                errorResponse.Message = invalidOperationException.Message;
                break;

            case KeyNotFoundException:
                errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.ErrorCode = "NOT_FOUND";
                errorResponse.Message = exception.Message;
                break;

            case TimeoutException:
                errorResponse.StatusCode = (int)HttpStatusCode.RequestTimeout;
                errorResponse.ErrorCode = "TIMEOUT";
                errorResponse.Message = "The request timed out. Please try again.";
                break;

            case TaskCanceledException:
                errorResponse.StatusCode = 499; // Client Closed Request
                errorResponse.ErrorCode = "REQUEST_CANCELLED";
                errorResponse.Message = "The request was cancelled.";
                break;

            default:
                errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.ErrorCode = "INTERNAL_SERVER_ERROR";
                errorResponse.Message = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "An unexpected error occurred. Please try again later.";
                break;
        }

        // Include details and stack trace only in development environment
        if (_environment.IsDevelopment())
        {
            errorResponse.Details = exception.ToString();
            errorResponse.StackTrace = exception.StackTrace;
        }

        return errorResponse;
    }

    private void LogException(Exception exception, ErrorResponse errorResponse)
    {
        var logMessage = $"Error {errorResponse.ErrorCode}: {errorResponse.Message} - Path: {errorResponse.Path}";

        switch (errorResponse.StatusCode)
        {
            case >= 500:
                _logger.LogError(exception, logMessage);
                break;
            case >= 400 and < 500:
                _logger.LogWarning(exception, logMessage);
                break;
            default:
                _logger.LogInformation(exception, logMessage);
                break;
        }
    }
}
