using System.ComponentModel.DataAnnotations;
using EWallet.Domain.Enums;

namespace EWallet.Domain.Entities;

public sealed class AuditLogs(long? actorUserId, long? transactionId, AuditLogAction action, AuditLogEntityType type, string? entityId, AuditLogOutcome outcome, string? description, string? correlationId)
{
    public long Id { get; private set; }

    // FK UserId
    public long? ActorUserId { get; private set; } = actorUserId;

    // FK TransactionId
    public long? TransactionId { get; private set; } = transactionId;

    [Required] public AuditLogAction Action { get; private set; } = action;
    [Required] public AuditLogEntityType Type { get; private set; } = type;
    [StringLength(60)] public string? EntityId { get; private set; } = entityId;
    [Required] public AuditLogOutcome Outcome { get; private set; } = outcome;
    [StringLength(500)] public string? Description { get; private set; } = description;
    [StringLength(100)] public string? CorrelationId { get; private set; } = correlationId;
    [Required] public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    // Relationship
    public Users? User { get; private set; } = null!;
    public Transactions? Transaction { get; private set; } = null!;
}
