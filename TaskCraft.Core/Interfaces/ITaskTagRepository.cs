using TaskCraft.Core.Entities;

namespace TaskCraft.Core.Interfaces;

public interface ITaskTagRepository : IRepository<TaskTag>
{
    Task<IEnumerable<TaskTag>> GetTagsByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskTag>> GetTasksByTagIdAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<TaskTag?> GetTaskTagAsync(Guid taskId, Guid tagId, CancellationToken cancellationToken = default);
    Task<bool> TaskHasTagAsync(Guid taskId, Guid tagId, CancellationToken cancellationToken = default);
}
