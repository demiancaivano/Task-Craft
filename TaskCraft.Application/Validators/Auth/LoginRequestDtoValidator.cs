using FluentValidation;
using TaskCraft.Application.DTOs.Auth;

namespace TaskCraft.Application.Validators.Auth;

/// <summary>
/// Validator for login requests
/// </summary>
public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters")
            .MaximumLength(100).WithMessage("Username cannot exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.IpAddress)
            .Matches(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$")
            .WithMessage("Invalid IP address format")
            .When(x => !string.IsNullOrWhiteSpace(x.IpAddress));
    }
}
