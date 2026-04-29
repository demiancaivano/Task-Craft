using TaskCraft.Core.Enums;
using TaskStatus = TaskCraft.Core.Enums.TaskStatus;

namespace TaskCraft.Core.Entities;

public class Task : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ParentTaskId { get; set; }

    public Project Project { get; set; } = null!;
    public Task? ParentTask { get; set; }
    public ICollection<Task> SubTasks { get; set; } = new List<Task>();
    public ICollection<TaskAssignment> Assignments { get; set; } = new List<TaskAssignment>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
