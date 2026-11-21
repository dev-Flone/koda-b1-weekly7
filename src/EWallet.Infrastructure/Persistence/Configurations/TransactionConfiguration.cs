
using EWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EWallet.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transactions>
{
    public void Configure(EntityTypeBuilder<Transactions> builder)
    {
        builder.ToTable("Transactions", "dbo", table =>
        {
            table.HasCheckConstraint(
                "CK_Transactions_Amounts",
                "[Amount] > 0");

            table.HasCheckConstraint(
                "CK_Transactions_Type",
                "[Type] IN ('TOPUP', 'TRANSFER')");

            table.HasCheckConstraint(
                "CK_Transactions_Status",
                "[Status] IN ('PENDING', 'SUCCESS', 'FAILED')");
        });

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnType("bigint")
            .UseIdentityColumn();

        builder.Property(t => t.ReferenceNo)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(t => t.ReferenceNo)
            .IsUnique();

        builder.HasOne(t => t.SenderWallet)
            .WithMany(w => w.SentTransactions)
            .HasForeignKey(t => t.SenderWalletId)
            .HasConstraintName("FK_Transactions_SenderWallet")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.SenderWalletId)
            .HasColumnType("bigint");

        builder.HasIndex(t => t.SenderWalletId);

        builder.HasOne(t => t.ReceiverWallet)
            .WithMany(w => w.ReceivedTransactions)
            .HasForeignKey(t => t.ReceiverWalletId)
            .HasConstraintName("FK_Transactions_ReceiverWallet")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.ReceiverWalletId)
            .HasColumnType("bigint");

        builder.HasIndex(t => t.ReceiverWalletId);

        builder.Property(t => t.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(t => t.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.CreatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(t => t.CompletedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.HasIndex(t => new
        {
            t.ReferenceNo,
            t.SenderWalletId,
            t.ReceiverWalletId,
            t.CreatedAtUtc
        }).HasDatabaseName(
            "IX_Transactions_ReferenceNo_SenderWalletId_ReceiverWalletId_CreatedAtUtc");
    }
}
