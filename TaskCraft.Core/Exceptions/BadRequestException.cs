namespace TaskCraft.Core.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated
/// </summary>
public class BadRequestException : BaseCustomException
{
    public BadRequestException(string message) 
        : base(message, 400, "BAD_REQUEST")
    {
    }
}
