using TaskCraft.Application.DTOs.User;
using TaskCraft.Application.Interfaces;
using TaskCraft.Application.Mappings;
using TaskCraft.Core.Entities;
using TaskCraft.Core.Exceptions;
using TaskCraft.Core.Interfaces;

namespace TaskCraft.Application.Services;

/// <summary>
/// Service implementation for user-related business operations
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            var errors = new Dictionary<string, string[]>
            {
                { "Email", new[] { "Email cannot be empty" } }
            };
            throw new ValidationException(errors);
        }

        return await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
    }

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            var errors = new Dictionary<string, string[]>
            {
                { "Username", new[] { "Username cannot be empty" } }
            };
            throw new ValidationException(errors);
        }

        return await _unitOfWork.Users.GetByUsernameAsync(username, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Users.GetAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(bool includeDeleted, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Users.GetAllAsync(includeDeleted, cancellationToken);
    }

    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        // Business validation
        var validationErrors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(user.Username))
            validationErrors.Add("Username", new[] { "Username is required" });

        if (string.IsNullOrWhiteSpace(user.Email))
            validationErrors.Add("Email", new[] { "Email is required" });

        if (validationErrors.Any())
            throw new ValidationException(validationErrors);

        // Check for duplicates
        if (await _unitOfWork.Users.EmailExistsAsync(user.Email, cancellationToken))
            throw new ConflictException($"Email '{user.Email}' is already registered");

        if (await _unitOfWork.Users.UsernameExistsAsync(user.Username, cancellationToken))
            throw new ConflictException($"Username '{user.Username}' is already taken");

        // Create user
        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<User> CreateUserAsync(CreateUserDto createUserDto, string password, CancellationToken cancellationToken = default)
    {
        var user = createUserDto.ToEntity();
        user.PasswordHash = password;
        return await CreateUserAsync(user, cancellationToken);
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        // Check if user exists
        var existingUser = await _unitOfWork.Users.GetByIdAsync(user.Id, cancellationToken);
        if (existingUser == null)
            throw new NotFoundException("User", user.Id);

        // Business validation
        var validationErrors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(user.Username))
            validationErrors.Add("Username", new[] { "Username is required" });

        if (string.IsNullOrWhiteSpace(user.Email))
            validationErrors.Add("Email", new[] { "Email is required" });

        if (validationErrors.Any())
            throw new ValidationException(validationErrors);

        // Check if email changed and if new email is available
        if (existingUser.Email != user.Email && 
            await _unitOfWork.Users.EmailExistsAsync(user.Email, cancellationToken))
            throw new ConflictException($"Email '{user.Email}' is already registered");

        // Check if username changed and if new username is available
        if (existingUser.Username != user.Username && 
            await _unitOfWork.Users.UsernameExistsAsync(user.Username, cancellationToken))
            throw new ConflictException($"Username '{user.Username}' is already taken");

        // Update user
        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<User> UpdateUserAsync(UpdateUserDto updateUserDto, string? hashedPassword = null, CancellationToken cancellationToken = default)
    {
        var existingUser = await _unitOfWork.Users.GetByIdAsync(updateUserDto.Id, cancellationToken);
        if (existingUser == null)
            throw new NotFoundException("User", updateUserDto.Id);

        existingUser.UpdateFromDto(updateUserDto, hashedPassword);

        return await UpdateUserAsync(existingUser, cancellationToken);
    }

    public async Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        if (user == null)
            throw new NotFoundException("User", id);

        // Soft delete
        await _unitOfWork.Users.SoftDeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return await _unitOfWork.Users.EmailExistsAsync(email, cancellationToken);
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        return await _unitOfWork.Users.UsernameExistsAsync(username, cancellationToken);
    }
}
