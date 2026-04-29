namespace TaskCraft.Application.DTOs.Project;

/// <summary>
/// DTO for returning project information to the client
/// Used in GET requests - lightweight version without nested collections
/// </summary>
public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int MemberCount { get; set; }
    public int TaskCount { get; set; }
}
