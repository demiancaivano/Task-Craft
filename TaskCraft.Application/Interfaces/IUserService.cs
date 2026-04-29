using TaskCraft.Application.DTOs.User;
using TaskCraft.Core.Entities;

namespace TaskCraft.Application.Interfaces;

/// <summary>
/// Service interface for user-related business operations
/// </summary>
public interface IUserService
{
    Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllUsersAsync(bool includeDeleted, CancellationToken cancellationToken = default);
    Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User> CreateUserAsync(CreateUserDto createUserDto, string password, CancellationToken cancellationToken = default);
    Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User> UpdateUserAsync(UpdateUserDto updateUserDto, string? hashedPassword = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
}
