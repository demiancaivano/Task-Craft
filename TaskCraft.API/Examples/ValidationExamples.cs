using TaskCraft.Application.DTOs.Auth;
using TaskCraft.Application.Validators.Auth;
using TaskCraft.Application.Validators.User;
using TaskCraft.Application.Validators.Task;
using TaskCraft.Application.Validators.Comment;
using TaskCraft.Application.Validators.Tag;

namespace TaskCraft.API.Examples;

/// <summary>
/// Examples demonstrating how FluentValidation works in TaskCraft API
/// These are NOT unit tests - they are educational examples
/// </summary>
public static class ValidationExamples
{
    /// <summary>
    /// Example 1: Valid registration - all rules pass
    /// </summary>
    public static void Example_ValidRegistration()
    {
        var validator = new RegisterRequestDtoValidator();
        var dto = new RegisterRequestDto
        {
            Username = "johndoe",
            Email = "john.doe@example.com",
            Password = "SecurePass123!",
            FirstName = "John",
            LastName = "Doe"
        };

        var result = validator.Validate(dto);

        // ✅ result.IsValid = true
        // ✅ result.Errors = empty
    }

    /// <summary>
    /// Example 2: Invalid registration - multiple validation errors
    /// </summary>
    public static void Example_InvalidRegistration()
    {
        var validator = new RegisterRequestDtoValidator();
        var dto = new RegisterRequestDto
        {
            Username = "ab",                    // ❌ Too short (min 3 chars)
            Email = "invalid-email",            // ❌ Invalid format
            Password = "weak",                  // ❌ Too short, missing uppercase, number, special char
            FirstName = "John123",              // ❌ Contains numbers
            LastName = ""                       // ❌ Required
        };

        var result = validator.Validate(dto);

        // ❌ result.IsValid = false
        // ❌ result.Errors contains:
        //    - Username: "Username must be at least 3 characters"
        //    - Email: "Invalid email format"
        //    - Password: "Password must be at least 8 characters"
        //    - Password: "Password must contain at least one uppercase letter"
        //    - Password: "Password must contain at least one number"
        //    - Password: "Password must contain at least one special character"
        //    - FirstName: "First name can only contain letters, spaces, hyphens, and apostrophes"
        //    - LastName: "Last name is required"
    }

    /// <summary>
    /// Example 3: Password validation - common mistakes
    /// </summary>
    public static void Example_WeakPasswords()
    {
        var validator = new RegisterRequestDtoValidator();

        // ❌ Too short
        var dto1 = new RegisterRequestDto { Password = "Pass1!" };
        // Error: "Password must be at least 8 characters"

        // ❌ No uppercase
        var dto2 = new RegisterRequestDto { Password = "password123!" };
        // Error: "Password must contain at least one uppercase letter"

        // ❌ No lowercase
        var dto3 = new RegisterRequestDto { Password = "PASSWORD123!" };
        // Error: "Password must contain at least one lowercase letter"

        // ❌ No number
        var dto4 = new RegisterRequestDto { Password = "Password!" };
        // Error: "Password must contain at least one number"

        // ❌ No special character
        var dto5 = new RegisterRequestDto { Password = "Password123" };
        // Error: "Password must contain at least one special character"

        // ✅ All requirements met
        var dto6 = new RegisterRequestDto { Password = "SecurePass123!" };
        // Valid!
    }

    /// <summary>
    /// Example 4: How validation errors appear in API responses
    /// </summary>
    public static string Example_ApiErrorResponse()
    {
        // When a POST /api/auth/register request fails validation,
        // the API returns a 400 Bad Request with this JSON structure:

        return @"
{
  ""type"": ""https://tools.ietf.org/html/rfc7231#section-6.5.1"",
  ""title"": ""One or more validation errors occurred."",
  ""status"": 400,
  ""errors"": {
    ""Username"": [
      ""Username must be at least 3 characters""
    ],
    ""Email"": [
      ""Email is required"",
      ""Invalid email format""
    ],
    ""Password"": [
      ""Password must be at least 8 characters"",
      ""Password must contain at least one uppercase letter"",
      ""Password must contain at least one number"",
      ""Password must contain at least one special character""
    ],
    ""FirstName"": [
      ""First name is required""
    ]
  }
}";
    }

    /// <summary>
    /// Example 5: Conditional validation - Anonymous users
    /// </summary>
    public static void Example_AnonymousUserValidation()
    {
        var validator = new CreateUserDtoValidator();

        // ❌ Non-anonymous user without password
        var dto1 = new TaskCraft.Application.DTOs.User.CreateUserDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = null,        // ❌ Required for non-anonymous
            IsAnonymous = false
        };
        // Error: "Password is required for non-anonymous users"

        // ✅ Anonymous user without password - OK!
        var dto2 = new TaskCraft.Application.DTOs.User.CreateUserDto
        {
            Username = "guest_123",
            Email = "guest@example.com",
            Password = null,        // ✅ Allowed for anonymous users
            IsAnonymous = true
        };
        // Valid!
    }

    /// <summary>
    /// Example 6: Date validation - Task dates
    /// </summary>
    public static void Example_TaskDateValidation()
    {
        var validator = new CreateTaskDtoValidator();

        // ❌ StartDate after DueDate
        var dto1 = new TaskCraft.Application.DTOs.Task.CreateTaskDto
        {
            Title = "My Task",
            StartDate = new DateTime(2024, 12, 31),
            DueDate = new DateTime(2024, 1, 1),    // ❌ Before start date
            ProjectId = Guid.NewGuid()
        };
        // Error: "Start date must be before or equal to due date"

        // ✅ Valid date range
        var dto2 = new TaskCraft.Application.DTOs.Task.CreateTaskDto
        {
            Title = "My Task",
            StartDate = new DateTime(2024, 1, 1),
            DueDate = new DateTime(2024, 12, 31),  // ✅ After start date
            ProjectId = Guid.NewGuid()
        };
        // Valid!
    }

    /// <summary>
    /// Example 7: Complex validation - Comment must have TaskId OR ProjectId
    /// </summary>
    public static void Example_CommentValidation()
    {
        var validator = new CreateCommentDtoValidator();

        // ❌ No TaskId or ProjectId
        var dto1 = new TaskCraft.Application.DTOs.Comment.CreateCommentDto
        {
            Content = "Great work!",
            UserId = Guid.NewGuid(),
            TaskId = null,
            ProjectId = null
        };
        // Error: "Either TaskId or ProjectId must be provided"

        // ✅ Has TaskId
        var dto2 = new TaskCraft.Application.DTOs.Comment.CreateCommentDto
        {
            Content = "Great work!",
            UserId = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),    // ✅ Valid
            ProjectId = null
        };
        // Valid!

        // ✅ Has ProjectId
        var dto3 = new TaskCraft.Application.DTOs.Comment.CreateCommentDto
        {
            Content = "Project looks good",
            UserId = Guid.NewGuid(),
            TaskId = null,
            ProjectId = Guid.NewGuid()  // ✅ Valid
        };
        // Valid!
    }

    /// <summary>
    /// Example 8: Regex validation - Tag color format
    /// </summary>
    public static void Example_TagColorValidation()
    {
        var validator = new CreateTagDtoValidator();

        // ❌ Invalid color formats
        var dto1 = new TaskCraft.Application.DTOs.Tag.CreateTagDto
        {
            Name = "Important",
            Color = "red",              // ❌ Must be hex format
            ProjectId = Guid.NewGuid()
        };
        // Error: "Color must be in hex format (e.g., #FF5733)"

        var dto2 = new TaskCraft.Application.DTOs.Tag.CreateTagDto
        {
            Name = "Important",
            Color = "#FF573",           // ❌ Only 5 hex digits
            ProjectId = Guid.NewGuid()
        };
        // Error: "Color must be in hex format (e.g., #FF5733)"

        // ✅ Valid hex color
        var dto3 = new TaskCraft.Application.DTOs.Tag.CreateTagDto
        {
            Name = "Important",
            Color = "#FF5733",          // ✅ Valid hex format
            ProjectId = Guid.NewGuid()
        };
        // Valid!

        // ✅ No color (optional)
        var dto4 = new TaskCraft.Application.DTOs.Tag.CreateTagDto
        {
            Name = "Important",
            Color = null,               // ✅ Optional field
            ProjectId = Guid.NewGuid()
        };
        // Valid!
    }
}
