# 🛡️ Validators Directory

This directory contains all **FluentValidation** validators for the TaskCraft application.

## 📁 Structure

```
Validators/
├── Auth/               # Authentication-related validators
│   ├── LoginRequestDtoValidator.cs
│   ├── RegisterRequestDtoValidator.cs
│   └── RefreshTokenRequestDtoValidator.cs
├── User/               # User management validators
│   ├── CreateUserDtoValidator.cs
│   └── UpdateUserDtoValidator.cs
├── Project/            # Project management validators
│   ├── CreateProjectDtoValidator.cs
│   ├── UpdateProjectDtoValidator.cs
│   └── AddProjectMemberDtoValidator.cs
├── Task/               # Task management validators
│   ├── CreateTaskDtoValidator.cs
│   ├── UpdateTaskDtoValidator.cs
│   └── AssignTaskDtoValidator.cs
├── Comment/            # Comment validators
│   └── CreateCommentDtoValidator.cs
└── Tag/                # Tag validators
    └── CreateTagDtoValidator.cs
```

## 🎯 Purpose

Each validator ensures that incoming data:
- Meets required format specifications
- Contains valid values and ranges
- Passes business rule checks
- Provides clear, actionable error messages

## 📖 Usage

Validators are automatically discovered and applied by ASP.NET Core when configured in `Program.cs`:

```csharp
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestDtoValidator>();
```

## ✅ Example Validator

```csharp
using FluentValidation;
using TaskCraft.Application.DTOs.Auth;

namespace TaskCraft.Application.Validators.Auth;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
}
```

## 📚 Documentation

For complete validation rules and guidelines, see:
- [Validation Guide](../../TaskCraft.API/Docs/VALIDATION_GUIDE.md)
- [FluentValidation Docs](https://docs.fluentvalidation.net/)

---

**Note**: All validators in this directory are automatically registered and applied to their corresponding DTOs.
