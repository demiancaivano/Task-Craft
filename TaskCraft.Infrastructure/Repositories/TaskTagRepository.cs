using Microsoft.EntityFrameworkCore;
using TaskCraft.Core.Entities;
using TaskCraft.Core.Interfaces;
using TaskCraft.Infrastructure.Data;

namespace TaskCraft.Infrastructure.Repositories;

public class TaskTagRepository : Repository<TaskTag>, ITaskTagRepository
{
    public TaskTagRepository(TaskCraftDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TaskTag>> GetTagsByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(tt => tt.Tag)
            .Where(tt => tt.TaskId == taskId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskTag>> GetTasksByTagIdAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(tt => tt.Task)
            .Where(tt => tt.TagId == tagId)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskTag?> GetTaskTagAsync(Guid taskId, Guid tagId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(tt => tt.TaskId == taskId && tt.TagId == tagId, cancellationToken);
    }

    public async Task<bool> TaskHasTagAsync(Guid taskId, Guid tagId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(tt => tt.TaskId == taskId && tt.TagId == tagId, cancellationToken);
    }
}
