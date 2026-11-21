using EWallet.Application.Dtos.AuditLogs;
using EWallet.Domain.Enums;
using EWallet.Domain.Entities;
using EWallet.Application.Repositories.Interfaces;
using EWallet.Application.Services.Interfaces;

namespace EWallet.Application.Services;

public sealed class AuditLogService(IAuditLogRepository auditLogRepository) : IAuditLogService
{
    public async Task<List<AuditLogResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var auditLogs = await auditLogRepository.GetAllAsync(cancellationToken);

        return [.. auditLogs.Select(MapToResponseDto)];
    }

    public async Task<AuditLogResponseDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var auditLog = await auditLogRepository.GetByIdAsync(id, cancellationToken);

        return auditLog is null ? null : MapToResponseDto(auditLog);
    }

    public async Task<List<AuditLogResponseDto>> GetByActorUserIdAsync(long actorUserId,
        CancellationToken cancellationToken)
    {
        var auditLogs = await auditLogRepository.GetByActorIdAsync(actorUserId, cancellationToken);

        return [.. auditLogs.Select(MapToResponseDto)];
    }

    public async Task<List<AuditLogResponseDto>> GetByTransactionIdAsync(long transactionId, CancellationToken cancellationToken)
    {
        var auditLogs = await auditLogRepository.GetByTransactionIdAsync(transactionId, cancellationToken);

        return [.. auditLogs.Select(MapToResponseDto)];
    }

    public async Task<List<AuditLogResponseDto>> GetByActionAsync(AuditLogAction action, CancellationToken cancellationToken)
    {
        var auditLogs = await auditLogRepository.GetByActionAsync(action, cancellationToken);

        return [.. auditLogs.Select(MapToResponseDto)];
    }

    public async Task<List<AuditLogResponseDto>> GetByEntityTypeAsync(AuditLogEntityType type, CancellationToken cancellationToken)
    {
        var auditLogs = await auditLogRepository.GetByEntityTypeAsync(type, cancellationToken);

        return [.. auditLogs.Select(MapToResponseDto)];
    }

    public async Task<List<AuditLogResponseDto>> GetByOutcomeAsync(AuditLogOutcome outcome, CancellationToken cancellationToken)
    {
        var auditLogs = await auditLogRepository.GetByOutcomeAsync(outcome, cancellationToken);

        return [.. auditLogs.Select(MapToResponseDto)];
    }

    public Task AddAsync(AuditLogs auditLog, CancellationToken cancellationToken)
    {
        return auditLogRepository.AddAsync(auditLog, cancellationToken);
    }

    private static AuditLogResponseDto MapToResponseDto(AuditLogs auditLog)
    {
        return new AuditLogResponseDto(
            auditLog.Id,
            auditLog.ActorUserId,
            auditLog.TransactionId,
            auditLog.Action.ToString(),
            auditLog.Type.ToString(),
            auditLog.EntityId,
            auditLog.Outcome.ToString(),
            auditLog.Description,
            auditLog.CorrelationId,
            auditLog.CreatedAtUtc
        );
    }
}


