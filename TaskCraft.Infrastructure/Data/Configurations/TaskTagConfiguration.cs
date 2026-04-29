using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskCraft.Core.Entities;

namespace TaskCraft.Infrastructure.Data.Configurations;

public class TaskTagConfiguration : IEntityTypeConfiguration<TaskTag>
{
    public void Configure(EntityTypeBuilder<TaskTag> builder)
    {
        builder.ToTable("TaskTags");

        builder.HasKey(tt => tt.Id);

        builder.HasOne(tt => tt.Task)
            .WithMany(t => t.TaskTags)
            .HasForeignKey(tt => tt.TaskId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(tt => tt.Tag)
            .WithMany(t => t.TaskTags)
            .HasForeignKey(tt => tt.TagId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(tt => new { tt.TaskId, tt.TagId })
            .IsUnique();

        builder.HasQueryFilter(tt => !tt.IsDeleted);
    }
}
