using TaskCraft.Application.DTOs.User;
using TaskCraft.Core.Entities;

namespace TaskCraft.Application.Mappings;

/// <summary>
/// Extension methods for mapping between User entity and User DTOs
/// </summary>
public static class UserMappingExtensions
{
    /// <summary>
    /// Converts User entity to UserDto
    /// Used when returning user data to the client
    /// </summary>
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            IsAnonymous = user.IsAnonymous,
            CreatedAt = user.CreatedAt
        };
    }

    /// <summary>
    /// Converts CreateUserDto to User entity
    /// Password hashing should be done before calling this method
    /// </summary>
    public static User ToEntity(this CreateUserDto dto, string? passwordHash = null)
    {
        return new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = passwordHash,
            Role = dto.Role,
            IsAnonymous = dto.IsAnonymous
        };
    }

    /// <summary>
    /// Updates an existing User entity with data from UpdateUserDto
    /// Preserves fields that shouldn't be modified (Id, CreatedAt, etc.)
    /// </summary>
    public static void UpdateFromDto(this User user, UpdateUserDto dto, string? newPasswordHash = null)
    {
        user.Username = dto.Username;
        user.Email = dto.Email;

        if (newPasswordHash != null)
        {
            user.PasswordHash = newPasswordHash;
        }
    }

    /// <summary>
    /// Converts a collection of User entities to UserDto list
    /// Useful for returning multiple users
    /// </summary>
    public static List<UserDto> ToDtoList(this IEnumerable<User> users)
    {
        return users.Select(u => u.ToDto()).ToList();
    }
}
