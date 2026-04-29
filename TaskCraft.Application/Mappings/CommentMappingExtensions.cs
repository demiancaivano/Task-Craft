using TaskCraft.Application.DTOs.Comment;
using TaskCraft.Core.Entities;

namespace TaskCraft.Application.Mappings;

/// <summary>
/// Extension methods for mapping between Comment entity and Comment DTOs
/// </summary>
public static class CommentMappingExtensions
{
    /// <summary>
    /// Converts Comment entity to CommentDto
    /// Includes denormalized username for convenience
    /// </summary>
    public static CommentDto ToDto(this Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            TaskId = comment.TaskId,
            ProjectId = comment.ProjectId,
            UserId = comment.UserId,
            Username = comment.User?.Username ?? "Unknown",
            CreatedAt = comment.CreatedAt
        };
    }

    /// <summary>
    /// Converts CreateCommentDto to Comment entity
    /// Either TaskId or ProjectId must be set (validated in service layer)
    /// </summary>
    public static Comment ToEntity(this CreateCommentDto dto)
    {
        return new Comment
        {
            Content = dto.Content,
            TaskId = dto.TaskId,
            ProjectId = dto.ProjectId,
            UserId = dto.UserId
        };
    }

    /// <summary>
    /// Converts a collection of Comment entities to CommentDto list
    /// </summary>
    public static List<CommentDto> ToDtoList(this IEnumerable<Comment> comments)
    {
        return comments.Select(c => c.ToDto()).ToList();
    }
}
