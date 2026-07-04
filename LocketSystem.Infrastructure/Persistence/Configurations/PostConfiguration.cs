using LocketMini.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocketMini.Infrastructure.Persistence.Configurations;

public sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> b)
    {
        b.ToTable("Posts");

        b.HasKey(p => p.PostId);
        b.Property(p => p.PostId)
            .HasColumnName("post_id")
            .UseIdentityColumn();

        b.Property(p => p.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        b.Property(p => p.Caption)
            .HasColumnName("caption")
            .HasColumnType("nvarchar(max)");

        b.Property(p => p.ImageUrl)
            .HasColumnName("image_url")
            .HasColumnType("nvarchar(max)");

        b.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

        // ── Navigation: Comments (1-N) ──────────────────────────────────
        b.HasMany(p => p.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Navigation: Likes (1-N) ─────────────────────────────────────
        b.HasMany(p => p.Likes)
            .WithOne(l => l.Post)
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index để query feed nhanh
        b.HasIndex(p => new { p.UserId, p.CreatedAt });
    }
}