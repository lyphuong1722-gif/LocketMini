using LocketMini.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocketMini.Infrastructure.Persistence.Configurations;

// ── Comment ────────────────────────────────────────────────────────────────────

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> b)
    {
        b.ToTable("Comments");

        b.HasKey(c => c.CommentId);
        b.Property(c => c.CommentId)
            .HasColumnName("comment_id")
            .UseIdentityColumn();

        b.Property(c => c.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        b.Property(c => c.PostId)
            .HasColumnName("post_id")
            .IsRequired();

        b.Property(c => c.Content)
            .HasColumnName("content")
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        b.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

        b.HasIndex(c => c.PostId);
    }
}

// ── Like ───────────────────────────────────────────────────────────────────────

public sealed class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> b)
    {
        b.ToTable("Likes");

        b.HasKey(l => l.LikeId);
        b.Property(l => l.LikeId)
            .HasColumnName("like_id")
            .UseIdentityColumn();

        b.Property(l => l.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        b.Property(l => l.PostId)
            .HasColumnName("post_id")
            .IsRequired();

        b.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

        // Một user chỉ like một post một lần
        b.HasIndex(l => new { l.UserId, l.PostId }).IsUnique();
    }
}

// ── Friend ─────────────────────────────────────────────────────────────────────

public sealed class FriendConfiguration : IEntityTypeConfiguration<Friend>
{
    public void Configure(EntityTypeBuilder<Friend> b)
    {
        b.ToTable("Friends");

        // Composite PK khớp với DB schema
        b.HasKey(f => new { f.UserId, f.FriendId });

        b.Property(f => f.UserId).HasColumnName("user_id");
        b.Property(f => f.FriendId).HasColumnName("friend_id");

        // FK tới FriendUser (navigation riêng để tránh xung đột cascade)
        b.HasOne(f => f.FriendUser)
            .WithMany()
            .HasForeignKey(f => f.FriendId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}