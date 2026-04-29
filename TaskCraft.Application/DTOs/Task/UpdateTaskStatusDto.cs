using System.ComponentModel.DataAnnotations;
using TaskCraft.Core.Enums;
using TaskStatus = TaskCraft.Core.Enums.TaskStatus;

namespace TaskCraft.Application.DTOs.Task;

/// <summary>
/// DTO for updating task status only
/// Used in PATCH requests for quick status changes
/// </summary>
public class UpdateTaskStatusDto
{
    [Required]
    public Guid TaskId { get; set; }

    [Required]
    public TaskStatus NewStatus { get; set; }
}
