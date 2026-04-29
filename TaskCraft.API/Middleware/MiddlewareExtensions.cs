namespace TaskCraft.API.Middleware;

/// <summary>
/// Extension methods for registering middleware in the application pipeline
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds global error handling middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}
