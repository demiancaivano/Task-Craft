using TaskCraft.Core.Entities;

namespace TaskCraft.Core.Interfaces;

public interface ITaskAssignmentRepository : IRepository<TaskAssignment>
{
    Task<IEnumerable<TaskAssignment>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskAssignment>> GetAssignmentsByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskAssignment>> GetAssignmentsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TaskAssignment?> GetAssignmentAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default);
    Task<bool> IsUserAssignedToTaskAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default);
    Task<int> GetAssignmentCountByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
}
