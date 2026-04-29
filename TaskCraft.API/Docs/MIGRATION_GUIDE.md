# Quick Migration Guide - Error Handling

## 🚀 Quick Reference for Migrating Existing Code

This guide provides quick find-and-replace patterns for updating your existing code to use the new error handling system.

---

## 📋 Step-by-Step Migration

### Step 1: Add Using Statement

**Add to top of service files:**
```csharp
using TaskCraft.Core.Exceptions;
```

---

### Step 2: Replace Exception Types

#### Pattern 1: Resource Not Found

**Old:**
```csharp
if (user == null)
    throw new KeyNotFoundException($"User with ID '{id}' not found");
```

**New:**
```csharp
if (user == null)
    throw new NotFoundException("User", id);
```

---

#### Pattern 2: Duplicate Resource

**Old:**
```csharp
if (emailExists)
    throw new InvalidOperationException($"Email '{email}' is already registered");
```

**New:**
```csharp
if (emailExists)
    throw new ConflictException($"Email '{email}' is already registered");
```

---

#### Pattern 3: Single Field Validation

**Old:**
```csharp
if (string.IsNullOrWhiteSpace(email))
    throw new ArgumentException("Email is required", nameof(email));
```

**New:**
```csharp
if (string.IsNullOrWhiteSpace(email))
{
    var errors = new Dictionary<string, string[]>
    {
        { "Email", new[] { "Email is required" } }
    };
    throw new ValidationException(errors);
}
```

---

#### Pattern 4: Multiple Field Validation

**Old:**
```csharp
if (string.IsNullOrWhiteSpace(user.Username))
    throw new ArgumentException("Username is required");

if (string.IsNullOrWhiteSpace(user.Email))
    throw new ArgumentException("Email is required");
```

**New:**
```csharp
var validationErrors = new Dictionary<string, string[]>();

if (string.IsNullOrWhiteSpace(user.Username))
    validationErrors.Add("Username", new[] { "Username is required" });

if (string.IsNullOrWhiteSpace(user.Email))
    validationErrors.Add("Email", new[] { "Email is required" });

if (validationErrors.Any())
    throw new ValidationException(validationErrors);
```

---

#### Pattern 5: Business Rule Violation

**Old:**
```csharp
if (task.Status == TaskStatus.Completed)
    throw new InvalidOperationException("Cannot modify a completed task");
```

**New:**
```csharp
if (task.Status == TaskStatus.Completed)
    throw new BadRequestException("Cannot modify a completed task");
```

---

#### Pattern 6: Authentication Failure

**Old:**
```csharp
if (user == null || !VerifyPassword(password))
    throw new UnauthorizedAccessException("Invalid credentials");
```

**New:**
```csharp
if (user == null || !VerifyPassword(password))
    throw new UnauthorizedException("Invalid credentials");
```

---

#### Pattern 7: Permission Check

**Old:**
```csharp
if (!isProjectMember)
    throw new UnauthorizedAccessException("You must be a project member");
```

**New:**
```csharp
if (!isProjectMember)
    throw new ForbiddenException("You must be a project member");
```

---

## 🔍 Quick Search & Replace

Use these patterns in Visual Studio (Ctrl+H with "Match case" enabled):

### Search Pattern 1: KeyNotFoundException
```
Find:    throw new KeyNotFoundException($"([^"]+) with ID '{([^']+)}' not found");
Replace: throw new NotFoundException("$1", $2);
```

### Search Pattern 2: InvalidOperationException for duplicates
```
Find:    throw new InvalidOperationException($"([^"]+) already exists");
Replace: throw new ConflictException("$1 already exists");
```

### Search Pattern 3: UnauthorizedAccessException
```
Find:    throw new UnauthorizedAccessException(
Replace: throw new UnauthorizedException(
```

---

## 📊 Exception Mapping Cheat Sheet

| Old Exception | New Exception | Use When |
|--------------|---------------|----------|
| `KeyNotFoundException` | `NotFoundException` | Resource doesn't exist |
| `ArgumentException` | `ValidationException` | Invalid input |
| `ArgumentNullException` | `ValidationException` | Null input |
| `InvalidOperationException` (duplicate) | `ConflictException` | Resource already exists |
| `InvalidOperationException` (rule) | `BadRequestException` | Business rule violated |
| `UnauthorizedAccessException` (auth) | `UnauthorizedException` | Not authenticated |
| `UnauthorizedAccessException` (perms) | `ForbiddenException` | No permission |

---

## ✅ Checklist for Each Service File

- [ ] Add `using TaskCraft.Core.Exceptions;`
- [ ] Replace `KeyNotFoundException` → `NotFoundException`
- [ ] Replace `ArgumentException` → `ValidationException`
- [ ] Replace `InvalidOperationException` → `ConflictException` or `BadRequestException`
- [ ] Replace `UnauthorizedAccessException` → `UnauthorizedException` or `ForbiddenException`
- [ ] Group multiple validation checks into single `ValidationException`
- [ ] Remove try-catch blocks from controllers
- [ ] Test all endpoints

---

## 🎯 Controller Migration

### Before
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetUser(Guid id)
{
    try
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }
    catch (KeyNotFoundException)
    {
        return NotFound();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error");
        return StatusCode(500);
    }
}
```

### After
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetUser(Guid id)
{
    var user = await _userService.GetUserByIdAsync(id);
    return Ok(user);
}
```

**Steps:**
1. Remove all try-catch blocks
2. Remove null checks that return NotFound()
3. Remove manual error logging
4. Let the middleware handle everything

---

## 🧪 Testing After Migration

Test these scenarios for each endpoint:

1. **404 Not Found**: Request non-existent resource
2. **400 Validation Error**: Send invalid data
3. **409 Conflict**: Try to create duplicate
4. **401 Unauthorized**: Invalid credentials
5. **403 Forbidden**: Insufficient permissions

**Example Test:**
```bash
# Test NotFoundException
curl -X GET http://localhost:5000/api/users/00000000-0000-0000-0000-000000000000

# Expected Response:
{
  "statusCode": 404,
  "errorCode": "NOT_FOUND",
  "message": "User with identifier '00000000-0000-0000-0000-000000000000' was not found.",
  "timestamp": "2024-01-15T10:30:00Z",
  "path": "/api/users/00000000-0000-0000-0000-000000000000"
}
```

---

## 💡 Pro Tips

1. **Batch Replace**: Use Visual Studio's "Replace All" carefully
2. **Test Incrementally**: Migrate one service at a time
3. **Use Git**: Commit after each service migration
4. **Check Logs**: Verify logging is working correctly
5. **Swagger Testing**: Use Swagger UI to test all scenarios

---

## 🚨 Common Mistakes to Avoid

❌ **Don't do this:**
```csharp
// Still catching in controller
try {
    await _service.DoSomething();
} catch (NotFoundException) {
    return NotFound();
}
```

✅ **Do this instead:**
```csharp
// Let middleware handle it
await _service.DoSomething();
```

---

❌ **Don't do this:**
```csharp
// Still using generic message
throw new NotFoundException("Resource not found");
```

✅ **Do this instead:**
```csharp
// Provide resource name and identifier
throw new NotFoundException("User", userId);
```

---

❌ **Don't do this:**
```csharp
// Separate validation checks
if (string.IsNullOrWhiteSpace(user.Name))
    throw new ValidationException("Name is required");
if (string.IsNullOrWhiteSpace(user.Email))
    throw new ValidationException("Email is required");
```

✅ **Do this instead:**
```csharp
// Group validation errors
var errors = new Dictionary<string, string[]>();
if (string.IsNullOrWhiteSpace(user.Name))
    errors.Add("Name", new[] { "Name is required" });
if (string.IsNullOrWhiteSpace(user.Email))
    errors.Add("Email", new[] { "Email is required" });
if (errors.Any())
    throw new ValidationException(errors);
```

---

## 📖 Additional Resources

- **Full Documentation**: `TaskCraft.API/Docs/ERROR_HANDLING.md`
- **Usage Examples**: `TaskCraft.API/Examples/ErrorHandlingExamples.cs`
- **Implementation Summary**: `TaskCraft.API/Docs/ERROR_HANDLING_IMPLEMENTATION_SUMMARY.md`

---

## ⏱️ Estimated Migration Time

- **Small service (< 10 methods)**: ~15 minutes
- **Medium service (10-20 methods)**: ~30 minutes
- **Large service (20+ methods)**: ~45 minutes
- **Full project**: ~2-3 hours

---

## 🎉 You're Done!

Once you've migrated all services:
1. Build the solution
2. Run all tests
3. Test all endpoints in Swagger
4. Deploy to development environment
5. Monitor logs for any issues

**Happy coding! 🚀**
