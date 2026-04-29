using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskCraft.Application.DTOs.Auth;
using TaskCraft.Application.Interfaces;

namespace TaskCraft.API.Controllers;

/// <summary>
/// Authentication and authorization endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticates a user and returns JWT tokens
    /// </summary>
    /// <param name="request">Login credentials (username/email and password)</param>
    /// <returns>JWT access token and refresh token</returns>
    /// <response code="200">Returns the JWT tokens</response>
    /// <response code="401">Invalid credentials</response>
    /// <response code="400">Invalid request format</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Registers a new user account
    /// </summary>
    /// <param name="request">User registration information</param>
    /// <returns>JWT access token and refresh token for the newly created user</returns>
    /// <response code="200">User successfully registered and authenticated</response>
    /// <response code="400">Invalid request or email/username already exists</response>
    /// <response code="409">User with provided email or username already exists</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LoginResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Refreshes an expired access token using a valid refresh token
    /// </summary>
    /// <param name="request">Refresh token request containing the refresh token</param>
    /// <returns>New JWT access token and refresh token</returns>
    /// <response code="200">Token successfully refreshed</response>
    /// <response code="401">Invalid or expired refresh token</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var response = await _authService.RefreshTokenAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Revokes a refresh token (logout)
    /// </summary>
    /// <param name="request">Refresh token to revoke</param>
    /// <returns>Success message</returns>
    /// <response code="200">Token successfully revoked</response>
    /// <response code="400">Invalid token</response>
    /// <response code="401">Unauthorized - requires authentication</response>
    [HttpPost("revoke")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> RevokeToken([FromBody] RefreshTokenRequestDto request)
    {
        request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        await _authService.RevokeTokenAsync(request.RefreshToken, request.IpAddress);
        return Ok(new { message = "Token revoked successfully" });
    }

    /// <summary>
    /// Validates if a refresh token is still active and valid
    /// </summary>
    /// <param name="request">Refresh token to validate</param>
    /// <returns>True if token is valid, false otherwise</returns>
    /// <response code="200">Returns validation result</response>
    [HttpPost("validate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> ValidateToken([FromBody] RefreshTokenRequestDto request)
    {
        var isValid = await _authService.ValidateTokenAsync(request.RefreshToken);
        return Ok(new { isValid });
    }
}
