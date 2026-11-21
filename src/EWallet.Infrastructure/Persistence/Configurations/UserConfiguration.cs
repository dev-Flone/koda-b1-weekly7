
using EWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EWallet.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<Users>
{
    public void Configure(EntityTypeBuilder<Users> builder)
    {
        builder.ToTable("Users", "dbo", table =>
        {
            table.HasCheckConstraint(
                "CK_Users_Status",
                "[Status] IN ('ACTIVE', 'SUSPENDED')");
        });

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnType("bigint")
            .UseIdentityColumn();

        builder.Property(u => u.UserName)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(u => u.UserName)
            .IsUnique();

        builder.Property(u => u.FullName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Phone)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(u => u.Phone)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.CreatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(u => u.UpdatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.HasIndex(u => new
        {
            u.UserName,
            u.CreatedAtUtc
        });
    }
}
