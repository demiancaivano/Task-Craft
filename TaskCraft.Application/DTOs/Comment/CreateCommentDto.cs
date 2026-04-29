using System.ComponentModel.DataAnnotations;

namespace TaskCraft.Application.DTOs.Comment;

/// <summary>
/// DTO for creating a new comment
/// Used in POST requests - must have either TaskId or ProjectId
/// </summary>
public class CreateCommentDto
{
    [Required(ErrorMessage = "Comment content is required")]
    [StringLength(2000, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 2000 characters")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "User ID is required")]
    public Guid UserId { get; set; }

    public Guid? TaskId { get; set; }

    public Guid? ProjectId { get; set; }
}
