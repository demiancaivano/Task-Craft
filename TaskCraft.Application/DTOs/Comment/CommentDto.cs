namespace TaskCraft.Application.DTOs.Comment;

/// <summary>
/// DTO for returning comment information to the client
/// Used in GET requests
/// </summary>
public class CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public Guid? TaskId { get; set; }
    public Guid? ProjectId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
