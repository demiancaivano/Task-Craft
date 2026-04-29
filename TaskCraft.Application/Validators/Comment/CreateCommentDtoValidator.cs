using FluentValidation;
using TaskCraft.Application.DTOs.Comment;

namespace TaskCraft.Application.Validators.Comment;

/// <summary>
/// Validator for creating a new comment
/// </summary>
public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required")
            .MinimumLength(1).WithMessage("Comment must be at least 1 character")
            .MaximumLength(2000).WithMessage("Comment cannot exceed 2000 characters");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        // At least one of TaskId or ProjectId must be provided
        RuleFor(x => x)
            .Must(x => x.TaskId.HasValue || x.ProjectId.HasValue)
            .WithMessage("Either TaskId or ProjectId must be provided");

        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID must be valid")
            .When(x => x.TaskId.HasValue);

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID must be valid")
            .When(x => x.ProjectId.HasValue);
    }
}
