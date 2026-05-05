using TaskCraft.Core.Enums;

namespace TaskCraft.Application.DTOs.User;

/// <summary>
/// DTO for returning user information to the client
/// Used in GET requests - excludes sensitive data like password
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public UserRole Role { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
