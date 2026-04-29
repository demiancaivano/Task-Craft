namespace TaskCraft.Core.Exceptions;

/// <summary>
/// Exception thrown when validation fails for business rules
/// </summary>
public class ValidationException : BaseCustomException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message) 
        : base(message, 400, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors) 
        : base("One or more validation errors occurred.", 
               400, 
               "VALIDATION_ERROR")
    {
        Errors = errors;
    }
}
