namespace TaskCraft.Core.Entities;

/// <summary>
/// Represents a refresh token for JWT authentication
/// Allows users to obtain new access tokens without re-authenticating
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>
    /// The actual token string (should be unique and cryptographically secure)
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The user this refresh token belongs to
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// When this refresh token expires and can no longer be used
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// When this refresh token was revoked (if manually invalidated)
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Optional: IP address that created this token (for security tracking)
    /// </summary>
    public string? CreatedByIp { get; set; }

    /// <summary>
    /// Optional: IP address that revoked this token
    /// </summary>
    public string? RevokedByIp { get; set; }

    /// <summary>
    /// Token that replaced this one (when refreshing)
    /// </summary>
    public string? ReplacedByToken { get; set; }

    /// <summary>
    /// Navigation property to User
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Helper property to check if token is active
    /// </summary>
    public bool IsActive => RevokedAt == null && !IsExpired;

    /// <summary>
    /// Helper property to check if token is expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
