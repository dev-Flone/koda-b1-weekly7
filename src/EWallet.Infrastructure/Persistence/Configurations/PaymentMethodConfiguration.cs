
using EWallet.Domain.Entities;
using EWallet.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EWallet.Infrastructure.Persistence.Configurations;

public class PaymentMethodConfiguration
    : IEntityTypeConfiguration<PaymentMethods>
{
    public void Configure(EntityTypeBuilder<PaymentMethods> builder)
    {
        builder.ToTable("PaymentMethods", "dbo", table =>
        {
            table.HasCheckConstraint(
                "CK_PaymentMethods_Code",
                "[Code] IN ('BANK_TRANSFER', 'QRIS')");
        });

        builder.HasKey(pm => pm.Id);

        // ID ditentukan secara eksplisit melalui HasData.
        builder.Property(pm => pm.Id)
            .ValueGeneratedNever();

        builder.Property(pm => pm.Code)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(pm => pm.Code)
            .IsUnique();

        builder.Property(pm => pm.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(pm => pm.ProviderName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(pm => pm.IsActive)
            .IsRequired();

        builder.Property(pm => pm.CreatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasIndex(pm => new
        {
            pm.Name,
            pm.IsActive,
            pm.CreatedAtUtc
        }).HasDatabaseName(
            "IX_PaymentMethods_Name_IsActive_CreatedAtUtc");

        builder.HasData(
            new
            {
                Id = 0,
                Code = PaymentMethodCode.BANK_TRANSFER,
                Name = "Bank Transfer",
                ProviderName = "Bank",
                IsActive = true,
                CreatedAtUtc = new DateTime(
                    2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = 1,
                Code = PaymentMethodCode.QRIS,
                Name = "QRIS",
                ProviderName = "QRIS",
                IsActive = true,
                CreatedAtUtc = new DateTime(
                    2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
