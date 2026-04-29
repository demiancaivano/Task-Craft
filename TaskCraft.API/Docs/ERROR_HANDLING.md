# Error Handling Middleware - TaskCraft API

## 📋 Overview

The Error Handling Middleware provides centralized exception handling for the TaskCraft API, converting exceptions into standardized JSON error responses with appropriate HTTP status codes.

## 🏗️ Architecture

### Components

1. **Custom Exceptions** (`TaskCraft.Core/Exceptions/`)
   - `BaseCustomException` - Base class for all custom exceptions
   - `NotFoundException` - 404 Not Found
   - `ValidationException` - 400 Bad Request (with validation errors)
   - `UnauthorizedException` - 401 Unauthorized
   - `ForbiddenException` - 403 Forbidden
   - `ConflictException` - 409 Conflict
   - `BadRequestException` - 400 Bad Request

2. **Error Response Model** (`TaskCraft.API/Models/ErrorResponse.cs`)
   - Standardized JSON structure for all error responses

3. **Middleware** (`TaskCraft.API/Middleware/ErrorHandlingMiddleware.cs`)
   - Catches all unhandled exceptions
   - Maps exceptions to appropriate HTTP status codes
   - Logs errors with appropriate severity levels
   - Returns consistent error responses

## 🚀 Usage

### In Services

```csharp
using TaskCraft.Core.Exceptions;

public class UserService : IUserService
{
    public async Task<User> GetUserByIdAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException("User", id);
        }

        return user;
    }

    public async Task CreateUserAsync(CreateUserDto dto)
    {
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(dto.Email);

        if (existingUser != null)
        {
            throw new ConflictException("A user with this email already exists");
        }

        // Validation example
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            var errors = new Dictionary<string, string[]>
            {
                { "Email", new[] { "Email is required" } },
                { "Password", new[] { "Password must be at least 8 characters" } }
            };
            throw new ValidationException(errors);
        }

        // Create user logic...
    }

    public async Task DeleteUserAsync(Guid id, Guid currentUserId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException("User", id);
        }

        if (id == currentUserId)
        {
            throw new BadRequestException("You cannot delete your own account");
        }

        // Permission check
        if (!await IsAdmin(currentUserId))
        {
            throw new ForbiddenException("Only administrators can delete users");
        }

        // Delete logic...
    }
}
```

### In Controllers

Controllers don't need try-catch blocks anymore! The middleware handles everything:

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetUser(Guid id)
{
    // No try-catch needed - middleware handles exceptions
    var user = await _userService.GetUserByIdAsync(id);
    return Ok(user);
}

[HttpPost]
public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
{
    // No try-catch needed
    var user = await _userService.CreateUserAsync(dto);
    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
}
```

## 📤 Error Response Format

### Standard Error Response

```json
{
  "statusCode": 404,
  "errorCode": "NOT_FOUND",
  "message": "User with identifier '123e4567-e89b-12d3-a456-426614174000' was not found.",
  "timestamp": "2024-01-15T10:30:00Z",
  "path": "/api/users/123e4567-e89b-12d3-a456-426614174000"
}
```

### Validation Error Response

```json
{
  "statusCode": 400,
  "errorCode": "VALIDATION_ERROR",
  "message": "One or more validation errors occurred.",
  "errors": {
    "Email": ["Email is required", "Email format is invalid"],
    "Password": ["Password must be at least 8 characters"]
  },
  "timestamp": "2024-01-15T10:30:00Z",
  "path": "/api/users"
}
```

### Development Environment Response

In development mode, additional debugging information is included:

```json
{
  "statusCode": 500,
  "errorCode": "INTERNAL_SERVER_ERROR",
  "message": "An error occurred while processing your request",
  "details": "System.InvalidOperationException: Sequence contains no elements...",
  "stackTrace": "   at System.Linq.Enumerable.First[TSource]...",
  "timestamp": "2024-01-15T10:30:00Z",
  "path": "/api/users"
}
```

## 🎯 Exception Mapping

| Exception Type | HTTP Status | Error Code |
|----------------|-------------|------------|
| `NotFoundException` | 404 | NOT_FOUND |
| `ValidationException` | 400 | VALIDATION_ERROR |
| `UnauthorizedException` | 401 | UNAUTHORIZED |
| `ForbiddenException` | 403 | FORBIDDEN |
| `ConflictException` | 409 | CONFLICT |
| `BadRequestException` | 400 | BAD_REQUEST |
| `ArgumentException` | 400 | INVALID_ARGUMENT |
| `ArgumentNullException` | 400 | NULL_ARGUMENT |
| `UnauthorizedAccessException` | 401 | UNAUTHORIZED_ACCESS |
| `InvalidOperationException` | 400 | INVALID_OPERATION |
| `KeyNotFoundException` | 404 | NOT_FOUND |
| `TimeoutException` | 408 | TIMEOUT |
| `TaskCanceledException` | 499 | REQUEST_CANCELLED |
| Other exceptions | 500 | INTERNAL_SERVER_ERROR |

## 📊 Logging

The middleware automatically logs exceptions with appropriate severity levels:

- **500-599 (Server Errors)**: `LogError` - Critical issues that need immediate attention
- **400-499 (Client Errors)**: `LogWarning` - Client errors and validation issues
- **Other**: `LogInformation` - General information

Log format:
```
Error {ErrorCode}: {Message} - Path: {RequestPath}
```

## 🔧 Configuration

The middleware is registered in `Program.cs`:

```csharp
// Must be one of the first middleware in the pipeline
app.UseErrorHandling();
```

**Important**: Place `UseErrorHandling()` before other middleware to catch exceptions from the entire pipeline.

## 🌍 Environment Differences

### Development
- Full exception details included in response
- Stack traces visible
- Detailed error messages

### Production
- Generic error messages for security
- No stack traces
- No internal implementation details
- Sensitive information hidden

## 🎨 Best Practices

1. **Use specific exceptions**: Prefer `NotFoundException` over generic `Exception`
2. **Provide context**: Include resource name and identifier when possible
3. **Validation errors**: Use `ValidationException` with field-specific errors
4. **Don't catch exceptions in controllers**: Let the middleware handle them
5. **Log additional context**: The middleware logs automatically, but you can add more context in services
6. **Security**: Never expose sensitive data in exception messages

## 📝 Example Scenarios

### Scenario 1: Resource Not Found
```csharp
var project = await _unitOfWork.Projects.GetByIdAsync(id);
if (project == null)
{
    throw new NotFoundException("Project", id);
}
```

### Scenario 2: Authorization
```csharp
var isMember = await _projectService.IsUserMemberAsync(projectId, userId);
if (!isMember)
{
    throw new ForbiddenException("You must be a project member to view tasks");
}
```

### Scenario 3: Business Rule Violation
```csharp
if (task.Status == TaskStatus.Completed)
{
    throw new BadRequestException("Cannot modify a completed task");
}
```

### Scenario 4: Duplicate Resource
```csharp
var existingProject = await _unitOfWork.Projects.GetByNameAsync(dto.Name);
if (existingProject != null)
{
    throw new ConflictException("A project with this name already exists");
}
```

## 🧪 Testing

When testing, you can verify that the correct exceptions are thrown:

```csharp
[Fact]
public async Task GetUserById_UserNotFound_ThrowsNotFoundException()
{
    // Arrange
    var userId = Guid.NewGuid();

    // Act & Assert
    await Assert.ThrowsAsync<NotFoundException>(
        () => _userService.GetUserByIdAsync(userId)
    );
}
```

## 🔄 Migration from Try-Catch

### Before (with try-catch in controller):
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetUser(Guid id)
{
    try
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting user");
        return StatusCode(500, new { message = "An error occurred" });
    }
}
```

### After (with middleware):
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetUser(Guid id)
{
    var user = await _userService.GetUserByIdAsync(id);
    return Ok(user);
}
```

Much cleaner! 🎉

## 📚 Additional Resources

- See `TaskCraft.API/Examples/ErrorHandlingExamples.cs` for more usage examples
- All custom exceptions are in `TaskCraft.Core/Exceptions/`
- Error response model: `TaskCraft.API/Models/ErrorResponse.cs`
- Middleware implementation: `TaskCraft.API/Middleware/ErrorHandlingMiddleware.cs`
