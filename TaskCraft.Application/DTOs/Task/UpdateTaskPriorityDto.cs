using System.ComponentModel.DataAnnotations;
using TaskCraft.Core.Enums;

namespace TaskCraft.Application.DTOs.Task;

/// <summary>
/// DTO for updating task priority only
/// Used in PATCH requests for quick priority changes
/// </summary>
public class UpdateTaskPriorityDto
{
    [Required]
    public Guid TaskId { get; set; }

    [Required]
    public TaskPriority NewPriority { get; set; }
}
