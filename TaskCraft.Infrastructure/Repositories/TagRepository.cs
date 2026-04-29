using Microsoft.EntityFrameworkCore;
using TaskCraft.Core.Entities;
using TaskCraft.Core.Interfaces;
using TaskCraft.Infrastructure.Data;

namespace TaskCraft.Infrastructure.Repositories;

public class TagRepository : Repository<Tag>, ITagRepository
{
    public TagRepository(TaskCraftDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Tag>> GetTagsByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tag?> GetTagByNameAsync(Guid projectId, string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Name == name, cancellationToken);
    }

    public async Task<bool> TagExistsInProjectAsync(Guid projectId, string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(t => t.ProjectId == projectId && t.Name == name, cancellationToken);
    }
}
