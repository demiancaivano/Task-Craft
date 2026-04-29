namespace TaskCraft.Core.Exceptions;

/// <summary>
/// Base class for all custom exceptions in the application
/// </summary>
public abstract class BaseCustomException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }

    protected BaseCustomException(string message, int statusCode, string errorCode) 
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    protected BaseCustomException(string message, int statusCode, string errorCode, Exception innerException) 
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}
