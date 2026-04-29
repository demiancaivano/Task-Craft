using TaskCraft.Core.Entities;
using SystemTask = System.Threading.Tasks.Task;

namespace TaskCraft.Core.Interfaces;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    System.Threading.Tasks.Task<RefreshToken?> GetByTokenAsync(string token);
    System.Threading.Tasks.Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
    SystemTask RevokeAsync(RefreshToken token, string? revokedByIp = null);
    SystemTask RevokeAllByUserIdAsync(Guid userId, string? revokedByIp = null);
}
