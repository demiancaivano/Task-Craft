using TaskCraft.Core.Entities;

namespace TaskCraft.Core.Interfaces;

public interface ITagRepository : IRepository<Tag>
{
    Task<IEnumerable<Tag>> GetTagsByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<Tag?> GetTagByNameAsync(Guid projectId, string name, CancellationToken cancellationToken = default);
    Task<bool> TagExistsInProjectAsync(Guid projectId, string name, CancellationToken cancellationToken = default);
}
