using TaskCraft.Application.DTOs.Auth;
using SystemTask = System.Threading.Tasks.Task;

namespace TaskCraft.Application.Interfaces;

public interface IAuthService
{
    System.Threading.Tasks.Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    System.Threading.Tasks.Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request);
    System.Threading.Tasks.Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    SystemTask RevokeTokenAsync(string token, string? ipAddress = null);
    System.Threading.Tasks.Task<bool> ValidateTokenAsync(string token);
}
