namespace TaskCraft.Core.Exceptions;

/// <summary>
/// Exception thrown when there is a conflict with the current state of the resource
/// </summary>
public class ConflictException : BaseCustomException
{
    public ConflictException(string message) 
        : base(message, 409, "CONFLICT")
    {
    }
}
