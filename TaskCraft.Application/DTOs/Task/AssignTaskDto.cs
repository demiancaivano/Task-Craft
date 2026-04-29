using System.ComponentModel.DataAnnotations;

namespace TaskCraft.Application.DTOs.Task;

/// <summary>
/// DTO for assigning a user to a task
/// Used in POST requests
/// </summary>
public class AssignTaskDto
{
    [Required(ErrorMessage = "Task ID is required")]
    public Guid TaskId { get; set; }

    [Required(ErrorMessage = "User ID is required")]
    public Guid UserId { get; set; }

    public Guid? AssignedBy { get; set; }
}
