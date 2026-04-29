using TaskCraft.Core.Enums;
using TaskStatus = TaskCraft.Core.Enums.TaskStatus;

namespace TaskCraft.Application.DTOs.Task;

/// <summary>
/// DTO for returning task information to the client
/// Used in GET requests - includes computed fields like IsOverdue
/// </summary>
public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid? ParentTaskId { get; set; }
    public string? ParentTaskTitle { get; set; }
    public DateTime CreatedAt { get; set; }
    public int AssigneeCount { get; set; }
    public int SubTaskCount { get; set; }
    public int CommentCount { get; set; }
    public bool IsOverdue { get; set; }
    public bool HasSubTasks { get; set; }
}
