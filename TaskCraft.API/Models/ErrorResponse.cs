namespace TaskCraft.API.Models;

/// <summary>
/// Standardized error response model for API responses
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Error code for identifying the type of error
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Detailed information about the error (only in development)
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Stack trace (only in development)
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Validation errors (field-specific errors)
    /// </summary>
    public IDictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// Timestamp when the error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Request path that generated the error
    /// </summary>
    public string? Path { get; set; }
}
