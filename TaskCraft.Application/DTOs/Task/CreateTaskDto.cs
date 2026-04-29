using System.ComponentModel.DataAnnotations;
using TaskCraft.Core.Enums;
using TaskStatus = TaskCraft.Core.Enums.TaskStatus;

namespace TaskCraft.Application.DTOs.Task;

/// <summary>
/// DTO for creating a new task
/// Used in POST requests
/// </summary>
public class CreateTaskDto
{
    [Required(ErrorMessage = "Task title is required")]
    [StringLength(300, MinimumLength = 3, ErrorMessage = "Task title must be between 3 and 300 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.ToDo;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime? StartDate { get; set; }

    public DateTime? DueDate { get; set; }

    [Required(ErrorMessage = "Project ID is required")]
    public Guid ProjectId { get; set; }

    public Guid? ParentTaskId { get; set; }
}
