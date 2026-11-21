
using EWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EWallet.Infrastructure.Persistence.Configurations;

public class TopUpRequestConfiguration
    : IEntityTypeConfiguration<TopUpRequests>
{
    public void Configure(EntityTypeBuilder<TopUpRequests> builder)
    {
        builder.ToTable("TopUpRequests", "dbo", table =>
        {
            table.HasCheckConstraint(
                "CK_TopUpRequests_Amount",
                "[Amount] > 0");

            table.HasCheckConstraint(
                "CK_TopUpRequests_Status",
                "[Status] IN ('PENDING', 'PAID', 'FAILED', 'EXPIRED')");
        });

        builder.HasKey(tr => tr.Id);

        builder.Property(tr => tr.Id)
            .HasColumnType("bigint")
            .UseIdentityColumn();

        builder.HasOne(tr => tr.Transaction)
            .WithOne(t => t.TopUpRequest)
            .HasForeignKey<TopUpRequests>(tr => tr.TransactionId)
            .HasConstraintName("FK_TopUpRequests_TransactionId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(tr => tr.TransactionId)
            .IsUnique();

        builder.HasOne(tr => tr.Wallet)
            .WithMany(w => w.TopUpRequests)
            .HasForeignKey(tr => tr.WalletId)
            .HasConstraintName("FK_TopUpRequests_WalletId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(tr => tr.WalletId);

        builder.HasOne(tr => tr.PaymentMethod)
            .WithMany(pm => pm.TopUpRequests)
            .HasForeignKey(tr => tr.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(tr => tr.PaymentMethodId);

        builder.Property(tr => tr.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(tr => tr.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(tr => tr.ProviderReference)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(tr => tr.CreatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(tr => tr.ExpiresAtUtc)
            .HasColumnType("datetime2");

        builder.Property(tr => tr.PaidAtUtc)
            .HasColumnType("datetime2");

        builder.HasIndex(tr => new
        {
            tr.TransactionId,
            tr.WalletId,
            tr.PaymentMethodId,
            tr.CreatedAtUtc
        }).HasDatabaseName(
            "TopUpRequests_TransactionId_WalletId_PamentMethodId_CreatedAtUtc");
    }
}
