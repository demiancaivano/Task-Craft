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
            .NotEmpty().WithMessage("Username or email is required")
            .MinimumLength(3).WithMessage("Username or email must be at least 3 characters")
            .MaximumLength(255).WithMessage("Username or email cannot exceed 255 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.IpAddress)
            .Matches(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$")
            .WithMessage("Invalid IP address format")
            .When(x => !string.IsNullOrWhiteSpace(x.IpAddress));
    }
}
