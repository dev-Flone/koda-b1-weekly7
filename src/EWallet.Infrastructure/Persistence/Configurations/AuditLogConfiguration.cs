
using EWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EWallet.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration
    : IEntityTypeConfiguration<AuditLogs>
{
    public void Configure(EntityTypeBuilder<AuditLogs> builder)
    {
        builder.ToTable("AuditLogs", "dbo", table =>
        {
            table.HasCheckConstraint(
                "CK_AuditLogs_Outcome",
                "[Outcome] IN ('SUCCESS', 'REJECTED', 'FAILED')");
        });

        builder.HasKey(al => al.Id);

        builder.Property(al => al.Id)
            .HasColumnType("bigint")
            .UseIdentityColumn();

        builder.HasOne(al => al.User)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(al => al.ActorUserId)
            .HasConstraintName("FK_AuditLogs_ActorUserId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(al => al.ActorUserId);

        builder.HasOne(al => al.Transaction)
            .WithMany(t => t.AuditLogs)
            .HasForeignKey(al => al.TransactionId)
            .HasConstraintName("FK_AuditLogs_TransactionId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(al => al.TransactionId);

        builder.Property(al => al.Action)
            .HasConversion<string>()
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(al => al.Type)
            .HasConversion<string>()
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(al => al.Outcome)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(al => al.EntityId)
            .HasMaxLength(60)
            .IsRequired(false);

        builder.Property(al => al.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(al => al.CorrelationId)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(al => al.CreatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasIndex(al => new
        {
            al.ActorUserId,
            al.TransactionId,
            al.CreatedAtUtc
        }).HasDatabaseName(
            "IX_ActorUserId_TransactionId_CreatedAtUtc");
    }
}
