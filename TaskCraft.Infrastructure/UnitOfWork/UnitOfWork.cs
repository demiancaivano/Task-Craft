using Microsoft.EntityFrameworkCore.Storage;
using TaskCraft.Core.Interfaces;
using TaskCraft.Infrastructure.Data;
using TaskCraft.Infrastructure.Repositories;

namespace TaskCraft.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly TaskCraftDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repository instances
    private IUserRepository? _users;
    private IProjectRepository? _projects;
    private IProjectMemberRepository? _projectMembers;
    private ITaskRepository? _tasks;
    private ITaskAssignmentRepository? _taskAssignments;
    private ICommentRepository? _comments;
    private ITagRepository? _tags;
    private ITaskTagRepository? _taskTags;
    private IRefreshTokenRepository? _refreshTokens;

    public UnitOfWork(TaskCraftDbContext context)
    {
        _context = context;
    }

    // Lazy initialization of repositories
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IProjectRepository Projects => _projects ??= new ProjectRepository(_context);
    public IProjectMemberRepository ProjectMembers => _projectMembers ??= new ProjectMemberRepository(_context);
    public ITaskRepository Tasks => _tasks ??= new TaskRepository(_context);
    public ITaskAssignmentRepository TaskAssignments => _taskAssignments ??= new TaskAssignmentRepository(_context);
    public ICommentRepository Comments => _comments ??= new CommentRepository(_context);
    public ITagRepository Tags => _tags ??= new TagRepository(_context);
    public ITaskTagRepository TaskTags => _taskTags ??= new TaskTagRepository(_context);
    public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
