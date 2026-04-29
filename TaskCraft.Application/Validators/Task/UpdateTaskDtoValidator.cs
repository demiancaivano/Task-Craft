using FluentValidation;
using TaskCraft.Application.DTOs.Task;

namespace TaskCraft.Application.Validators.Task;

/// <summary>
/// Validator for updating task information
/// </summary>
public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
{
    public UpdateTaskDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required")
            .MinimumLength(3).WithMessage("Task title must be at least 3 characters")
            .MaximumLength(300).WithMessage("Task title cannot exceed 300 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid task status specified");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid task priority specified");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.DueDate ?? DateTime.MaxValue)
            .WithMessage("Start date must be before or equal to due date")
            .When(x => x.StartDate.HasValue && x.DueDate.HasValue);

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(x => x.StartDate ?? DateTime.MinValue)
            .WithMessage("Due date must be after or equal to start date")
            .When(x => x.StartDate.HasValue && x.DueDate.HasValue);
    }
}
