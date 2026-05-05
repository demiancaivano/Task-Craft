using FluentValidation;
using TaskCraft.Application.DTOs.Project;

namespace TaskCraft.Application.Validators.Project;

/// <summary>
/// Validator for adding a member to a project
/// </summary>
public class AddProjectMemberDtoValidator : AbstractValidator<AddProjectMemberDto>
{
    public AddProjectMemberDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid project role specified");
    }
}
