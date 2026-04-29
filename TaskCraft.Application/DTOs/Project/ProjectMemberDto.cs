using TaskCraft.Core.Enums;

namespace TaskCraft.Application.DTOs.Project;

/// <summary>
/// DTO for returning project member information
/// Used in GET requests to list project members
/// </summary>
public class ProjectMemberDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public ProjectRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
}
