namespace TaskCraft.Application.DTOs.Task;

/// <summary>
/// DTO for returning task assignment information
/// Used in GET requests
/// </summary>
public class TaskAssignmentDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public Guid? AssignedBy { get; set; }
    public string? AssignedByUsername { get; set; }
}
