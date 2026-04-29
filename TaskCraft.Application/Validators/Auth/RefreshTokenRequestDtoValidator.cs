using FluentValidation;
using TaskCraft.Application.DTOs.Auth;

namespace TaskCraft.Application.Validators.Auth;

/// <summary>
/// Validator for refresh token requests
/// </summary>
public class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshTokenRequestDtoValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required")
            .MinimumLength(32).WithMessage("Invalid refresh token format");
    }
}
