
using EWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EWallet.Infrastructure.Persistence.Configurations;

public class WalletMutationConfiguration
    : IEntityTypeConfiguration<WalletMutations>
{
    public void Configure(EntityTypeBuilder<WalletMutations> builder)
    {
        builder.ToTable("WalletMutations", "dbo", table =>
        {
            table.HasCheckConstraint(
                "CK_WalletMutations_Direction",
                "[Direction] IN ('IN', 'OUT')");

            table.HasCheckConstraint(
                "CK_WalletMutations_Amount",
                "[Amount] > 0");
        });

        builder.HasKey(wm => wm.Id);

        builder.Property(wm => wm.Id)
            .HasColumnType("bigint")
            .UseIdentityColumn();

        builder.HasOne(wm => wm.Transaction)
            .WithMany(t => t.WalletMutations)
            .HasForeignKey(wm => wm.TransactionId)
            .HasConstraintName("FK_WalletMutations_Transactions")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(wm => wm.TransactionId);

        builder.HasOne(wm => wm.Wallets)
            .WithMany(w => w.WalletMutations)
            .HasForeignKey(wm => wm.WalletId)
            .HasConstraintName("FK_WalletMutations_Wallets")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(wm => wm.WalletId);

        builder.Property(wm => wm.Direction)
            .HasConversion<string>()
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(wm => wm.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(wm => wm.BalanceBefore)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(wm => wm.BalanceAfter)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(wm => wm.CreatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasIndex(wm => new
        {
            wm.TransactionId,
            wm.WalletId,
            wm.CreatedAtUtc
        }).HasDatabaseName(
            "IX_WalletMutations_TransactionId_WalletId_CreatedAtUtc");
    }
}
