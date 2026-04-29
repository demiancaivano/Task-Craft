using FluentValidation;
using TaskCraft.Application.DTOs.Project;

namespace TaskCraft.Application.Validators.Project;

/// <summary>
/// Validator for updating project information
/// </summary>
public class UpdateProjectDtoValidator : AbstractValidator<UpdateProjectDto>
{
    public UpdateProjectDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required")
            .MinimumLength(3).WithMessage("Project name must be at least 3 characters")
            .MaximumLength(200).WithMessage("Project name cannot exceed 200 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_\.]+$").WithMessage("Project name can only contain letters, numbers, spaces, hyphens, underscores, and periods");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}
