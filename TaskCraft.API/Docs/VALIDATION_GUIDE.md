# 🛡️ Validation Layer - TaskCraft API

## 📋 Overview

TaskCraft uses **FluentValidation** to provide comprehensive, centralized validation for all DTOs. This ensures data integrity, security, and provides clear, actionable error messages to clients.

---

## ✅ Benefits

- **🔒 Security**: Validates data before it reaches business logic
- **🎯 Consistency**: Single source of truth for all validation rules
- **📝 Clear Errors**: Structured, specific error messages for frontend
- **🧪 Testable**: Easy to unit test validation logic
- **📚 Documentation**: Swagger automatically shows validation requirements
- **🏗️ Clean Architecture**: Separates validation from business logic

---

## 🏗️ Architecture

```
TaskCraft.Application/
├── Validators/
│   ├── Auth/
│   │   ├── LoginRequestDtoValidator.cs
│   │   ├── RegisterRequestDtoValidator.cs
│   │   └── RefreshTokenRequestDtoValidator.cs
│   ├── User/
│   │   ├── CreateUserDtoValidator.cs
│   │   └── UpdateUserDtoValidator.cs
│   ├── Project/
│   │   ├── CreateProjectDtoValidator.cs
│   │   ├── UpdateProjectDtoValidator.cs
│   │   └── AddProjectMemberDtoValidator.cs
│   ├── Task/
│   │   ├── CreateTaskDtoValidator.cs
│   │   ├── UpdateTaskDtoValidator.cs
│   │   └── AssignTaskDtoValidator.cs
│   ├── Comment/
│   │   └── CreateCommentDtoValidator.cs
│   └── Tag/
│       └── CreateTagDtoValidator.cs
```

---

## 🚀 How It Works

### 1. Automatic Validation

FluentValidation is configured to run **automatically** on all controller actions:

```csharp
// Program.cs
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestDtoValidator>();
```

### 2. Controller Usage

No changes needed in controllers! Validation happens automatically:

```csharp
[HttpPost("register")]
public async Task<ActionResult<UserDto>> Register([FromBody] RegisterRequestDto dto)
{
    // ✅ At this point, dto is already validated
    // If validation fails, a 400 Bad Request is returned automatically

    var user = await _authService.RegisterAsync(dto);
    return Ok(user);
}
```

### 3. Error Response Format

When validation fails, clients receive a structured JSON response:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": [
      "Email is required",
      "Invalid email format"
    ],
    "Password": [
      "Password must be at least 8 characters",
      "Password must contain at least one uppercase letter",
      "Password must contain at least one number",
      "Password must contain at least one special character"
    ]
  }
}
```

---

## 📚 Validation Rules

### 🔐 Authentication

#### **RegisterRequestDto**
- **Username**: 3-100 chars, alphanumeric + `_-`, required
- **Email**: Valid email format, max 255 chars, required
- **Password**: 8-100 chars, must contain:
  - At least 1 uppercase letter
  - At least 1 lowercase letter
  - At least 1 number
  - At least 1 special character
- **FirstName**: Required, max 100 chars, letters only
- **LastName**: Required, max 100 chars, letters only
- **IpAddress**: Optional, valid IPv4 format

#### **LoginRequestDto**
- **Username**: 3-100 chars, required
- **Password**: Min 6 chars, required
- **IpAddress**: Optional, valid IPv4 format

---

### 👤 Users

#### **CreateUserDto**
- **Username**: 3-100 chars, alphanumeric + `_-`, required
- **Email**: Valid email, max 255 chars, required
- **Password**: Required for non-anonymous users, 8+ chars with complexity
- **Role**: Valid enum value (User, Admin, etc.)
- **IsAnonymous**: Boolean, required

#### **UpdateUserDto**
- **Id**: GUID, required
- **Username**: 3-100 chars, alphanumeric + `_-`, required
- **Email**: Valid email, max 255 chars, required
- **NewPassword**: Optional, 8+ chars with complexity if provided

---

### 📁 Projects

#### **CreateProjectDto**
- **Name**: 3-200 chars, alphanumeric + spaces + `-_.`, required
- **Description**: Optional, max 1000 chars
- **OwnerId**: GUID, required

#### **UpdateProjectDto**
- **Id**: GUID, required
- **Name**: 3-200 chars, required
- **Description**: Optional, max 1000 chars

#### **AddProjectMemberDto**
- **ProjectId**: GUID, required
- **UserId**: GUID, required
- **Role**: Valid ProjectRole enum, required

---

### ✅ Tasks

#### **CreateTaskDto**
- **Title**: 3-300 chars, required
- **Description**: Optional, max 2000 chars
- **Status**: Valid TaskStatus enum (ToDo, InProgress, Done, etc.)
- **Priority**: Valid TaskPriority enum (Low, Medium, High, Urgent)
- **StartDate**: Optional, must be ≤ DueDate
- **DueDate**: Optional, must be ≥ StartDate
- **ProjectId**: GUID, required
- **ParentTaskId**: Optional GUID

#### **UpdateTaskDto**
- **Id**: GUID, required
- **Title**: 3-300 chars, required
- **Description**: Optional, max 2000 chars
- **Status**: Valid TaskStatus enum
- **Priority**: Valid TaskPriority enum
- **StartDate/DueDate**: Date validation (StartDate ≤ DueDate)

#### **AssignTaskDto**
- **TaskId**: GUID, required
- **UserId**: GUID, required
- **AssignedBy**: Optional GUID

---

### 💬 Comments

#### **CreateCommentDto**
- **Content**: 1-2000 chars, required
- **UserId**: GUID, required
- **TaskId or ProjectId**: At least one must be provided

---

### 🏷️ Tags

#### **CreateTagDto**
- **Name**: 1-50 chars, alphanumeric + spaces + `-_`, required
- **Color**: Optional, hex format `#RRGGBB` (e.g., `#FF5733`)
- **ProjectId**: GUID, required

---

## 🧪 Testing Validators

Validators can be easily unit tested:

```csharp
[Fact]
public void RegisterDto_Should_Fail_When_Password_Too_Short()
{
    // Arrange
    var validator = new RegisterRequestDtoValidator();
    var dto = new RegisterRequestDto
    {
        Username = "testuser",
        Email = "test@example.com",
        Password = "Short1!", // Only 7 chars
        FirstName = "John",
        LastName = "Doe"
    };

    // Act
    var result = validator.Validate(dto);

    // Assert
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.PropertyName == "Password");
}
```

---

## 🎨 Frontend Integration

### React/JavaScript Example

```javascript
try {
  const response = await fetch('/api/auth/register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      username: 'user',
      email: 'invalid-email', // ❌ Invalid
      password: 'weak',        // ❌ Too weak
      firstName: 'John',
      lastName: 'Doe'
    })
  });

  if (!response.ok) {
    const error = await response.json();

    // Display validation errors
    console.log(error.errors);
    // {
    //   "Email": ["Invalid email format"],
    //   "Password": [
    //     "Password must be at least 8 characters",
    //     "Password must contain at least one uppercase letter",
    //     ...
    //   ]
    // }
  }
} catch (err) {
  console.error('Request failed:', err);
}
```

### Display Errors in Forms

```javascript
function RegisterForm() {
  const [errors, setErrors] = useState({});

  const handleSubmit = async (formData) => {
    try {
      const response = await api.post('/auth/register', formData);
      // Success!
    } catch (err) {
      if (err.response?.status === 400) {
        // Validation errors
        setErrors(err.response.data.errors);
      }
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <input name="email" />
      {errors.Email && <span className="error">{errors.Email[0]}</span>}

      <input name="password" type="password" />
      {errors.Password && (
        <ul className="error-list">
          {errors.Password.map((err, i) => <li key={i}>{err}</li>)}
        </ul>
      )}
    </form>
  );
}
```

---

## 🔧 Adding New Validators

### Step 1: Create Validator Class

```csharp
using FluentValidation;
using TaskCraft.Application.DTOs.YourModule;

namespace TaskCraft.Application.Validators.YourModule;

public class YourDtoValidator : AbstractValidator<YourDto>
{
    public YourDtoValidator()
    {
        RuleFor(x => x.PropertyName)
            .NotEmpty().WithMessage("Property is required")
            .MinimumLength(3).WithMessage("Minimum 3 characters");
    }
}
```

### Step 2: That's It!

FluentValidation automatically discovers and registers all validators in the assembly. No additional configuration needed!

---

## 📖 Common Validation Rules

```csharp
// Required field
RuleFor(x => x.Field).NotEmpty();

// String length
RuleFor(x => x.Field).MinimumLength(3).MaximumLength(100);

// Email
RuleFor(x => x.Email).EmailAddress();

// Regex pattern
RuleFor(x => x.Field).Matches(@"^[a-zA-Z]+$");

// Custom validation
RuleFor(x => x.Field).Must(BeValidValue).WithMessage("Custom message");

// Conditional validation
RuleFor(x => x.Field).NotEmpty().When(x => x.OtherField == true);

// Enum validation
RuleFor(x => x.Status).IsInEnum();

// Date comparisons
RuleFor(x => x.StartDate).LessThan(x => x.EndDate);

// Cross-field validation
RuleFor(x => x).Must(dto => dto.Field1 != dto.Field2);
```

---

## 🐛 Troubleshooting

### Validation Not Running?

1. Ensure `AddFluentValidationAutoValidation()` is called in `Program.cs`
2. Validator class must inherit from `AbstractValidator<TDto>`
3. Validator must be in the same assembly registered with `AddValidatorsFromAssembly()`

### Custom Error Messages Not Showing?

Use `.WithMessage()` after each rule:

```csharp
RuleFor(x => x.Email)
    .NotEmpty().WithMessage("Email is required")
    .EmailAddress().WithMessage("Invalid email format");
```

### Swagger Not Showing Validation Rules?

FluentValidation automatically integrates with Swagger when properly configured. Ensure:
- `FluentValidation.AspNetCore` package is installed
- Validators are registered in DI container

---

## 📚 Additional Resources

- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [ASP.NET Core Validation](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation)
- [Frontend Integration Guide](./FRONTEND_INTEGRATION.md)

---

**✨ Validation Layer Successfully Implemented!**

*Your API is now protected with comprehensive, maintainable, and user-friendly validation.*
