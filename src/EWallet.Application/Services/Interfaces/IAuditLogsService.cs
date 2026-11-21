using EWallet.Application.Dtos.AuditLogs;
using EWallet.Domain.Enums;
using EWallet.Domain.Entities;

namespace EWallet.Application.Services.Interfaces;

public interface IAuditLogService
{
    Task<List<AuditLogResponseDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<AuditLogResponseDto?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<List<AuditLogResponseDto>> GetByActorUserIdAsync(long actorUserId, CancellationToken cancellationToken);

    Task<List<AuditLogResponseDto>> GetByTransactionIdAsync(long transactionId, CancellationToken cancellationToken);

    Task<List<AuditLogResponseDto>> GetByActionAsync(AuditLogAction action, CancellationToken cancellationToken);

    Task<List<AuditLogResponseDto>> GetByEntityTypeAsync(AuditLogEntityType type, CancellationToken cancellationToken);

    Task<List<AuditLogResponseDto>> GetByOutcomeAsync(AuditLogOutcome outcome, CancellationToken cancellationToken);

    Task AddAsync(AuditLogs auditLog, CancellationToken cancellationToken);
}


