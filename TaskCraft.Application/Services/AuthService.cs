using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskCraft.Application.DTOs.Auth;
using TaskCraft.Application.Interfaces;
using TaskCraft.Core.Entities;
using TaskCraft.Core.Exceptions;
using TaskCraft.Core.Interfaces;
using SystemTask = System.Threading.Tasks.Task;

namespace TaskCraft.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async System.Threading.Tasks.Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var loginIdentifier = request.Username.Trim();
        var users = await _unitOfWork.Users.GetAllAsync();
        var user = users.FirstOrDefault(u =>
            !u.IsDeleted &&
            (string.Equals(u.Username, loginIdentifier, StringComparison.OrdinalIgnoreCase) ||
             string.Equals(u.Email, loginIdentifier, StringComparison.OrdinalIgnoreCase)));

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid username/email or password");
        }

        var accessToken = GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshToken(user.Id, request.IpAddress);

        return new LoginResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiration = DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes()),
            RefreshTokenExpiration = refreshToken.ExpiresAt
        };
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        if (users.Any(u => u.Username == request.Username))
        {
            throw new ConflictException("Username already exists");
        }

        if (users.Any(u => u.Email == request.Email))
        {
            throw new ConflictException("Email already exists");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CommitAsync();

        var accessToken = GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshToken(user.Id, request.IpAddress);

        return new LoginResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiration = DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes()),
            RefreshTokenExpiration = refreshToken.ExpiresAt
        };
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);

        if (refreshToken == null || !refreshToken.IsActive)
        {
            throw new UnauthorizedException("Invalid or expired refresh token");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(refreshToken.UserId);
        if (user == null || user.IsDeleted)
        {
            throw new UnauthorizedException("User not found");
        }

        var newRefreshToken = await GenerateRefreshToken(user.Id, request.IpAddress);

        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedByIp = request.IpAddress;
        refreshToken.ReplacedByToken = newRefreshToken.Token;
        await _unitOfWork.RefreshTokens.UpdateAsync(refreshToken);
        await _unitOfWork.CommitAsync();

        var accessToken = GenerateAccessToken(user);

        return new LoginResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            AccessTokenExpiration = DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes()),
            RefreshTokenExpiration = newRefreshToken.ExpiresAt
        };
    }

    public async SystemTask RevokeTokenAsync(string token, string? ipAddress = null)
    {
        var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(token);

        if (refreshToken == null || !refreshToken.IsActive)
        {
            throw new BadRequestException("Invalid token");
        }

        await _unitOfWork.RefreshTokens.RevokeAsync(refreshToken, ipAddress);
        await _unitOfWork.CommitAsync();
    }

    public async System.Threading.Tasks.Task<bool> ValidateTokenAsync(string token)
    {
        var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(token);
        return refreshToken != null && refreshToken.IsActive;
    }

    private string GenerateAccessToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtValue("SecretKey")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: GetJwtValue("Issuer"),
            audience: GetJwtValue("Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes()),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async System.Threading.Tasks.Task<RefreshToken> GenerateRefreshToken(Guid userId, string? ipAddress)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = GenerateRandomToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenExpirationDays()),
            CreatedByIp = ipAddress
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
        await _unitOfWork.CommitAsync();

        return refreshToken;
    }

    private static string GenerateRandomToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private int GetAccessTokenExpirationMinutes()
    {
        return int.Parse(GetJwtValue("AccessTokenExpirationMinutes", "15"));
    }

    private int GetRefreshTokenExpirationDays()
    {
        return int.Parse(GetJwtValue("RefreshTokenExpirationDays", "7"));
    }

    private string GetJwtValue(string key, string? defaultValue = null)
    {
        var value = _configuration[$"JwtSettings:{key}"] ?? _configuration[$"Jwt:{key}"] ?? defaultValue;

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"JWT setting '{key}' is not configured.");
        }

        return value;
    }
}
