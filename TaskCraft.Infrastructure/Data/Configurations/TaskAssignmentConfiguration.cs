using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskCraft.Core.Entities;

namespace TaskCraft.Infrastructure.Data.Configurations;

public class TaskAssignmentConfiguration : IEntityTypeConfiguration<TaskAssignment>
{
    public void Configure(EntityTypeBuilder<TaskAssignment> builder)
    {
        builder.ToTable("TaskAssignments");

        builder.HasKey(ta => ta.Id);

        builder.HasOne(ta => ta.User)
            .WithMany(u => u.TaskAssignments)
            .HasForeignKey(ta => ta.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ta => ta.Task)
            .WithMany(t => t.Assignments)
            .HasForeignKey(ta => ta.TaskId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(ta => new { ta.UserId, ta.TaskId })
            .IsUnique();

        builder.HasQueryFilter(ta => !ta.IsDeleted);
    }
}
