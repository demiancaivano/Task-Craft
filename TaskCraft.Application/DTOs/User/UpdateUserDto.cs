using System.ComponentModel.DataAnnotations;
using TaskCraft.Core.Enums;

namespace TaskCraft.Application.DTOs.User;

/// <summary>
/// DTO for updating user information
/// Used in PUT/PATCH requests - password update is optional
/// </summary>
public class UpdateUserDto
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;

    [StringLength(500, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string? NewPassword { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public UserRole Role { get; set; } = UserRole.User;
}
