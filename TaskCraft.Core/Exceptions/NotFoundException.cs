namespace TaskCraft.Core.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found
/// </summary>
public class NotFoundException : BaseCustomException
{
    public NotFoundException(string message) 
        : base(message, 404, "NOT_FOUND")
    {
    }

    public NotFoundException(string resourceName, object key) 
        : base($"{resourceName} with identifier '{key}' was not found.", 
               404, 
               "NOT_FOUND")
    {
    }
}
