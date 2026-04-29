namespace TaskCraft.Core.Exceptions;

/// <summary>
/// Exception thrown when a user is not authorized to perform an operation
/// </summary>
public class UnauthorizedException : BaseCustomException
{
    public UnauthorizedException(string message = "You are not authorized to perform this operation.") 
        : base(message, 401, "UNAUTHORIZED")
    {
    }
}
