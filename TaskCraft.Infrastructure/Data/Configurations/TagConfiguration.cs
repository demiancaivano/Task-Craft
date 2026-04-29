using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskCraft.Core.Entities;

namespace TaskCraft.Infrastructure.Data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Color)
            .HasMaxLength(7);

        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tags)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.TaskTags)
            .WithOne(tt => tt.Tag)
            .HasForeignKey(tt => tt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => new { t.ProjectId, t.Name })
            .IsUnique();

        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
