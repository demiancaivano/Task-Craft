using Microsoft.EntityFrameworkCore;
using TaskCraft.Core.Entities;
using TaskCraft.Core.Interfaces;
using TaskCraft.Infrastructure.Data;
using SystemTask = System.Threading.Tasks.Task;

namespace TaskCraft.Infrastructure.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(TaskCraftDbContext context) : base(context)
    {
    }

    public async System.Threading.Tasks.Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.Set<RefreshToken>()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async System.Threading.Tasks.Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
    {
        return await _context.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public async SystemTask RevokeAsync(RefreshToken token, string? revokedByIp = null)
    {
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = revokedByIp;
        await UpdateAsync(token);
    }

    public async SystemTask RevokeAllByUserIdAsync(Guid userId, string? revokedByIp = null)
    {
        var tokens = await _context.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = revokedByIp;
        }

        await _context.SaveChangesAsync();
    }
}
