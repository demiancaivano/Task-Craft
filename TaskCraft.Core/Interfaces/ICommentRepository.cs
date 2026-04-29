using TaskCraft.Core.Entities;

namespace TaskCraft.Core.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetCommentsByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comment>> GetCommentsByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetCommentCountByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<int> GetCommentCountByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
}
