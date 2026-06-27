using LocketMini.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocketMini.Infrastructure.Persistence.Configurations;

public sealed class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> b)
    {
        b.ToTable("Profiles");

        b.HasKey(p => p.ProfileId);
        b.Property(p => p.ProfileId).UseIdentityColumn();

        b.Property(p => p.UserId).IsRequired();

        b.Property(p => p.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(100);

        b.Property(p => p.Bio)
            .HasColumnName("bio")
            .HasColumnType("nvarchar(max)");

        b.HasIndex(p => p.UserId).IsUnique();
    }
}
