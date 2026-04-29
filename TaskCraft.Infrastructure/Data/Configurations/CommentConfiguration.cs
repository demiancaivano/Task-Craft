using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskCraft.Core.Entities;

namespace TaskCraft.Infrastructure.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.Project)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.ProjectId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasCheckConstraint(
            "CK_Comment_TaskOrProject",
            "(TaskId IS NOT NULL AND ProjectId IS NULL) OR (TaskId IS NULL AND ProjectId IS NOT NULL)");

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
