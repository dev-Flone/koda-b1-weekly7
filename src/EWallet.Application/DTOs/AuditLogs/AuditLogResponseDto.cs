namespace EWallet.Application.Dtos.AuditLogs;

public sealed record AuditLogResponseDto
{
    public long Id { get; private set; }
    public long? ActorUserId { get; private set; }
    public long? TransactionId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public string? EntityId { get; private set; }
    public string Outcome { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? CorrelationId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public AuditLogResponseDto(
        long id,
        long? actorUserId,
        long? transactionId,
        string action,
        string type,
        string? entityId,
        string outcome,
        string? description,
        string? correlationId,
        DateTime createdAtUtc)
    {
        Id = id;
        ActorUserId = actorUserId;
        TransactionId = transactionId;
        Action = action;
        Type = type;
        EntityId = entityId;
        Outcome = outcome;
        Description = description;
        CorrelationId = correlationId;
        CreatedAtUtc = createdAtUtc;
    }
}


