namespace TaskCraft.Application.DTOs.Auth;

public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
}
