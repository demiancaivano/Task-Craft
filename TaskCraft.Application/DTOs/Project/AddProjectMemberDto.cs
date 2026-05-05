using System.ComponentModel.DataAnnotations;
using TaskCraft.Core.Enums;

namespace TaskCraft.Application.DTOs.Project;

/// <summary>
/// DTO for adding a member to a project
/// Used in POST requests to add users to projects
/// </summary>
public class AddProjectMemberDto
{
    public Guid ProjectId { get; set; }

    [Required(ErrorMessage = "User ID is required")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public ProjectRole Role { get; set; }
}
