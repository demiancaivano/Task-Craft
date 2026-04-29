namespace TaskCraft.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IProjectRepository Projects { get; }
    IProjectMemberRepository ProjectMembers { get; }
    ITaskRepository Tasks { get; }
    ITaskAssignmentRepository TaskAssignments { get; }
    ICommentRepository Comments { get; }
    ITagRepository Tags { get; }
    ITaskTagRepository TaskTags { get; }
    IRefreshTokenRepository RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
