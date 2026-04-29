namespace TaskCraft.Core.Exceptions;

/// <summary>
/// Exception thrown when a user does not have permission to access a resource
/// </summary>
public class ForbiddenException : BaseCustomException
{
    public ForbiddenException(string message = "You do not have permission to access this resource.") 
        : base(message, 403, "FORBIDDEN")
    {
    }
}
