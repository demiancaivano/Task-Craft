using TaskCraft.Core.Exceptions;

namespace TaskCraft.API.Examples;

/// <summary>
/// Examples of how to use custom exceptions in services and controllers
/// This file is for documentation purposes and can be deleted
/// </summary>
public class ErrorHandlingExamples
{
    // Example 1: NotFoundException
    public async Task ExampleNotFoundException()
    {
        // Simple message
        throw new NotFoundException("User not found");

        // With resource name and key
        var userId = Guid.NewGuid();
        throw new NotFoundException("User", userId);
    }

    // Example 2: ValidationException
    public async Task ExampleValidationException()
    {
        // Simple validation error
        throw new ValidationException("Email format is invalid");

        // Multiple validation errors
        var errors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required", "Email format is invalid" } },
            { "Password", new[] { "Password must be at least 8 characters" } }
        };
        throw new ValidationException(errors);
    }

    // Example 3: UnauthorizedException
    public async Task ExampleUnauthorizedException()
    {
        // Default message
        throw new UnauthorizedException();

        // Custom message
        throw new UnauthorizedException("Invalid credentials provided");
    }

    // Example 4: ForbiddenException
    public async Task ExampleForbiddenException()
    {
        // Default message
        throw new ForbiddenException();

        // Custom message
        throw new ForbiddenException("You must be a project owner to perform this action");
    }

    // Example 5: ConflictException
    public async Task ExampleConflictException()
    {
        throw new ConflictException("A user with this email already exists");
    }

    // Example 6: BadRequestException
    public async Task ExampleBadRequestException()
    {
        throw new BadRequestException("Task priority cannot be changed after completion");
    }

    // Example 7: Real service usage
    public class UserServiceExample
    {
        public async Task<object> GetUserByIdAsync(Guid id)
        {
            // Simulate database call
            object? user = null;

            if (user == null)
            {
                throw new NotFoundException("User", id);
            }

            return user;
        }

        public async Task CreateUserAsync(string email, string password)
        {
            // Check if email already exists
            bool emailExists = true; // Simulate check

            if (emailExists)
            {
                throw new ConflictException("A user with this email already exists");
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(email))
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "Email", new[] { "Email is required" } }
                };
                throw new ValidationException(errors);
            }
        }

        public async Task DeleteUserAsync(Guid id, Guid currentUserId)
        {
            // Check if user exists
            object? user = null;
            if (user == null)
            {
                throw new NotFoundException("User", id);
            }

            // Check if user is deleting themselves
            if (id == currentUserId)
            {
                throw new BadRequestException("You cannot delete your own account");
            }

            // Check permissions
            bool isAdmin = false; // Check if current user is admin
            if (!isAdmin)
            {
                throw new ForbiddenException("Only administrators can delete user accounts");
            }
        }
    }
}
