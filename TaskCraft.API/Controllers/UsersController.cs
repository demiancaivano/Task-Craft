using Microsoft.AspNetCore.Mvc;
using TaskCraft.Application.DTOs;
using TaskCraft.Application.DTOs.User;
using TaskCraft.Application.Interfaces;
using TaskCraft.Application.Mappings;
using TaskCraft.Core.Interfaces;

namespace TaskCraft.API.Controllers;

/// <summary>
/// User management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        IUnitOfWork unitOfWork,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a paginated list of users
    /// </summary>
    /// <param name="page">Page number (1-based, default: 1)</param>
    /// <param name="pageSize">Number of users per page (default: 20, max: 100)</param>
    /// <param name="includeDeleted">Include soft-deleted users (default: false)</param>
    /// <returns>Paginated list of users</returns>
    /// <response code="200">Returns the paginated list of users</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserDto>>> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool includeDeleted = false)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (users, totalCount) = await _unitOfWork.Users.GetPagedAsync(page, pageSize, includeDeleted);
        var userDtos = users.Where(u => !u.IsAnonymous).ToDtoList();

        _logger.LogInformation("Retrieved {Count} users (page: {Page}, pageSize: {PageSize})",
            userDtos.Count, page, pageSize);

        return Ok(new PagedResult<UserDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        });
    }

    /// <summary>
    /// Retrieves a specific user by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <returns>User details</returns>
    /// <response code="200">Returns the user</response>
    /// <response code="404">User not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        var userDto = user.ToDto();
        _logger.LogInformation("Retrieved user {UserId}", id);

        return Ok(userDto);
    }

    /// <summary>
    /// Retrieves a user by their email address
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <returns>User details</returns>
    /// <response code="200">Returns the user</response>
    /// <response code="404">User not found</response>
    [HttpGet("email/{email}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email);
        var userDto = user.ToDto();
        _logger.LogInformation("Retrieved user by email {Email}", email);

        return Ok(userDto);
    }

    /// <summary>
    /// Retrieves a user by their username
    /// </summary>
    /// <param name="username">User's username</param>
    /// <returns>User details</returns>
    /// <response code="200">Returns the user</response>
    /// <response code="404">User not found</response>
    [HttpGet("username/{username}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(username);
        var userDto = user.ToDto();
        _logger.LogInformation("Retrieved user by username {Username}", username);

        return Ok(userDto);
    }

    /// <summary>
    /// Creates a new user account
    /// </summary>
    /// <param name="createUserDto">User registration details</param>
    /// <returns>Newly created user</returns>
    /// <response code="201">User successfully created</response>
    /// <response code="400">Invalid request or validation errors</response>
    /// <response code="409">User with email or username already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(createUserDto.Password))
                return BadRequest(new { message = "Password is required" });

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
            var createdUser = await _userService.CreateUserAsync(createUserDto, hashedPassword);
            await _unitOfWork.SaveChangesAsync();

            var userDto = createdUser.ToDto();
            _logger.LogInformation("Created new user {UserId} with username {Username}", 
                userDto.Id, userDto.Username);

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = userDto.Id },
                userDto);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User creation failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, new { message = "An error occurred while creating the user" });
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="updateUserDto">Updated user data</param>
    /// <returns>Updated user details</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            if (id != updateUserDto.Id)
            {
                return BadRequest(new { message = "ID in URL does not match ID in request body" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string? hashedPassword = null;
            if (!string.IsNullOrEmpty(updateUserDto.NewPassword))
            {
                hashedPassword = BCrypt.Net.BCrypt.HashPassword(updateUserDto.NewPassword);
            }

            var updatedUser = await _userService.UpdateUserAsync(updateUserDto, hashedPassword);
            await _unitOfWork.SaveChangesAsync();

            var userDto = updatedUser.ToDto();
            _logger.LogInformation("Updated user {UserId}", id);

            return Ok(userDto);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User {UserId} not found for update", id);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User update failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the user" });
        }
    }

    /// <summary>
    /// Soft delete a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Soft deleted user {UserId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User {UserId} not found for deletion", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the user" });
        }
    }
}
