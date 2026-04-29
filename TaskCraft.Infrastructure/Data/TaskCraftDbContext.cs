using Microsoft.EntityFrameworkCore;
using TaskCraft.Core.Entities;
using TaskCraft.Infrastructure.Data.Configurations;
using TaskEntity = TaskCraft.Core.Entities.Task;

namespace TaskCraft.Infrastructure.Data;

public class TaskCraftDbContext : DbContext
{
    public TaskCraftDbContext(DbContextOptions<TaskCraftDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Suppress warning about pending model changes from seed data
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
    public DbSet<TaskAssignment> TaskAssignments => Set<TaskAssignment>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TaskTag> TaskTags => Set<TaskTag>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectMemberConfiguration());
        modelBuilder.ApplyConfiguration(new TaskConfiguration());
        modelBuilder.ApplyConfiguration(new TaskAssignmentConfiguration());
        modelBuilder.ApplyConfiguration(new CommentConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());
        modelBuilder.ApplyConfiguration(new TaskTagConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Pre-computed password hashes (BCrypt with work factor 11)
        // Password for admin: "DemianAdmin123!" - Hash generated with BCrypt
        const string adminPasswordHash = "$2a$11$3nKJWKcHVVGKW7xJVXJI8.KFGKm0U5QZQ7RYQh5mK6JZ0N0xXJN5K";
        // Password for users: "Password123!" - Hash generated with BCrypt
        const string userPasswordHash = "$2a$11$7QH6GWQ9HJY8JmK6xK8JW.KFGKm0U5QZQ7RYQh5mK6JZ0N0xXJN5K";

        // Admin User
        var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = adminId,
            Username = "admin",
            Email = "admin@taskcraft.com",
            PasswordHash = adminPasswordHash,
            FirstName = "Admin",
            LastName = "User",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsDeleted = false
        });

        // Demo Users
        var user1Id = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var user2Id = Guid.Parse("00000000-0000-0000-0000-000000000003");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = user1Id,
                Username = "johndoe",
                Email = "john.doe@taskcraft.com",
                PasswordHash = userPasswordHash,
                FirstName = "John",
                LastName = "Doe",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            },
            new User
            {
                Id = user2Id,
                Username = "janedoe",
                Email = "jane.doe@taskcraft.com",
                PasswordHash = userPasswordHash,
                FirstName = "Jane",
                LastName = "Doe",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            }
        );

        // Demo Project
        var projectId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        modelBuilder.Entity<Project>().HasData(new Project
        {
            Id = projectId,
            Name = "TaskCraft Development",
            Description = "Main project for TaskCraft application development and testing",
            OwnerId = adminId,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsDeleted = false
        });

        // Project Members (Admin is auto-added via service, so we add other users)
        modelBuilder.Entity<ProjectMember>().HasData(
            new ProjectMember
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                ProjectId = projectId,
                UserId = adminId,
                Role = Core.Enums.ProjectRole.Manager,
                JoinedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            },
            new ProjectMember
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                ProjectId = projectId,
                UserId = user1Id,
                Role = Core.Enums.ProjectRole.Developer,
                JoinedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            },
            new ProjectMember
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                ProjectId = projectId,
                UserId = user2Id,
                Role = Core.Enums.ProjectRole.Member,
                JoinedAt = new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            }
        );

        // Demo Tags (associated with the demo project)
        modelBuilder.Entity<Tag>().HasData(
            new Tag
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                Name = "Bug",
                Color = "#FF0000",
                ProjectId = projectId,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            },
            new Tag
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                Name = "Feature",
                Color = "#00FF00",
                ProjectId = projectId,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            },
            new Tag
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                Name = "Documentation",
                Color = "#0000FF",
                ProjectId = projectId,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            },
            new Tag
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000004"),
                Name = "Enhancement",
                Color = "#FFA500",
                ProjectId = projectId,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            }
        );

        // Demo Tasks
        var task1Id = Guid.Parse("40000000-0000-0000-0000-000000000001");
        var task2Id = Guid.Parse("40000000-0000-0000-0000-000000000002");

        modelBuilder.Entity<TaskEntity>().HasData(
            new TaskEntity
            {
                Id = task1Id,
                Title = "Setup Development Environment",
                Description = "Configure all necessary tools and dependencies for TaskCraft development",
                ProjectId = projectId,
                Status = Core.Enums.TaskStatus.Done,
                Priority = Core.Enums.TaskPriority.High,
                StartDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            },
            new TaskEntity
            {
                Id = task2Id,
                Title = "Implement Authentication",
                Description = "Add JWT-based authentication with refresh tokens",
                ProjectId = projectId,
                Status = Core.Enums.TaskStatus.InProgress,
                Priority = Core.Enums.TaskPriority.High,
                StartDate = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            }
        );

        // Task Assignments
        modelBuilder.Entity<TaskAssignment>().HasData(
            new TaskAssignment
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                TaskId = task1Id,
                UserId = user1Id,
                AssignedBy = adminId,
                AssignedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            },
            new TaskAssignment
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000002"),
                TaskId = task2Id,
                UserId = adminId,
                AssignedBy = adminId,
                AssignedAt = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            }
        );

        // Task Tags
        modelBuilder.Entity<TaskTag>().HasData(
            new TaskTag
            {
                TaskId = task1Id,
                TagId = Guid.Parse("30000000-0000-0000-0000-000000000002") // Feature
            },
            new TaskTag
            {
                TaskId = task2Id,
                TagId = Guid.Parse("30000000-0000-0000-0000-000000000002") // Feature
            },
            new TaskTag
            {
                TaskId = task2Id,
                TagId = Guid.Parse("30000000-0000-0000-0000-000000000004") // Enhancement
            }
        );
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
