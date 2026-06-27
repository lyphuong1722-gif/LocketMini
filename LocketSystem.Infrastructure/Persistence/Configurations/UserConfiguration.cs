using LocketMini.Domain.Entities;
using LocketMini.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LocketMini.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users");

        b.HasKey(u => u.UserId);
        b.Property(u => u.UserId)
            .UseIdentityColumn();

        // ── Value Object: Username ─────────────────────────────────────────
        b.Property(u => u.Username)
            .HasConversion(
                v => v.Value,
                v => Username.Create(v))
            .HasColumnName("username")
            .HasMaxLength(50)
            .IsRequired();

        b.HasIndex(u => u.Username)
            .IsUnique();

        // ── Value Object: Password ────────────────────────────────────────
        b.Property(u => u.Password)
            .HasConversion(
                v => v.HashedValue,
                v => Password.Create(v))
            .HasColumnName("password")
            .HasMaxLength(255)
            .IsRequired();

        // ── Navigation: Profile (1-1) ──────────────────────────────────────
        b.HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<Profile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Navigation: Posts (1-N) ────────────────────────────────────────
        b.HasMany(u => u.Posts)
            .WithOne(p => p.Owner)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Navigation: Comments (1-N) ─────────────────────────────────────
        b.HasMany(u => u.Comments)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // ── Navigation: Likes (1-N) ────────────────────────────────────────
        b.HasMany(u => u.Likes)
            .WithOne(l => l.User)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // ── Navigation: Friends (1-N) ──────────────────────────────────────
        b.HasMany(u => u.Friends)
            .WithOne(f => f.User)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
