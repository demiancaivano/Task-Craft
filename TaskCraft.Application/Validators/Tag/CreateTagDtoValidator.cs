using FluentValidation;
using TaskCraft.Application.DTOs.Tag;

namespace TaskCraft.Application.Validators.Tag;

/// <summary>
/// Validator for creating a new tag
/// </summary>
public class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
{
    public CreateTagDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tag name is required")
            .MinimumLength(1).WithMessage("Tag name must be at least 1 character")
            .MaximumLength(50).WithMessage("Tag name cannot exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$").WithMessage("Tag name can only contain letters, numbers, spaces, hyphens, and underscores");

        RuleFor(x => x.Color)
            .Matches(@"^#([A-Fa-f0-9]{6})$")
            .WithMessage("Color must be in hex format (e.g., #FF5733)")
            .When(x => !string.IsNullOrWhiteSpace(x.Color));

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");
    }
}
