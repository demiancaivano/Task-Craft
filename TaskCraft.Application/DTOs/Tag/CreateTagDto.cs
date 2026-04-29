using System.ComponentModel.DataAnnotations;

namespace TaskCraft.Application.DTOs.Tag;

/// <summary>
/// DTO for creating a new tag
/// Used in POST requests
/// </summary>
public class CreateTagDto
{
    [Required(ErrorMessage = "Tag name is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Tag name must be between 1 and 50 characters")]
    public string Name { get; set; } = string.Empty;

    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "Color must be in hex format (e.g., #FF5733)")]
    public string? Color { get; set; }

    [Required(ErrorMessage = "Project ID is required")]
    public Guid ProjectId { get; set; }
}
