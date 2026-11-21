
using EWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EWallet.Infrastructure.Persistence.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallets>
{
    public void Configure(EntityTypeBuilder<Wallets> builder)
    {
        builder.ToTable("Wallets", "dbo", table =>
        {
            table.HasCheckConstraint(
                "CK_Wallets_Balance",
                "[Balance] >= 0");

            table.HasCheckConstraint(
                "CK_Wallets_Status",
                "[Status] IN ('ACTIVE', 'FROZEN', 'CLOSED')");
        });

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .HasColumnType("bigint")
            .UseIdentityColumn();

        builder.HasOne(w => w.Users)
            .WithOne(u => u.Wallets)
            .HasForeignKey<Wallets>(w => w.UserId)
            .HasConstraintName("FK_Wallets_Users")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(w => w.UserId)
            .HasColumnType("bigint");

        builder.HasIndex(w => w.UserId)
            .IsUnique();

        builder.Property(w => w.WalletNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(w => w.WalletNumber)
            .IsUnique();

        builder.Property(w => w.Balance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(w => w.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(w => w.RowVersion)
            .IsRowVersion()
            .IsRequired();

        builder.Property(w => w.CreatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(w => w.UpdatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.HasIndex(w => new
        {
            w.UserId,
            w.WalletNumber,
            w.CreatedAtUtc
        }).HasDatabaseName(
            "IX_Wallets_UserId_WalletNumber_CreatedAtUtc");
    }
}
