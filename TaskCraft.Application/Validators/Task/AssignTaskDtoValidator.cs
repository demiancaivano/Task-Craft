using FluentValidation;
using TaskCraft.Application.DTOs.Task;

namespace TaskCraft.Application.Validators.Task;

/// <summary>
/// Validator for assigning a user to a task
/// </summary>
public class AssignTaskDtoValidator : AbstractValidator<AssignTaskDto>
{
    public AssignTaskDtoValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        // AssignedBy is optional, but if provided should be valid
        RuleFor(x => x.AssignedBy)
            .NotEmpty().WithMessage("Assigned by user ID must be valid")
            .When(x => x.AssignedBy.HasValue);
    }
}
