using EWallet.Domain.Enums;
using EWallet.Domain.Entities;

namespace EWallet.Application.Repositories.Interfaces;

public interface IAuditLogRepository
{
    Task<List<AuditLogs>> GetAllAsync(CancellationToken cancellationToken);
    Task<AuditLogs?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<List<AuditLogs>> GetByActorIdAsync(long actorId, CancellationToken cancellationToken);
    Task<List<AuditLogs>> GetByTransactionIdAsync(long transactionId, CancellationToken cancellationToken);
    Task<List<AuditLogs>> GetByActionAsync(AuditLogAction action, CancellationToken cancellationToken);
    Task<List<AuditLogs>> GetByEntityTypeAsync(AuditLogEntityType type, CancellationToken cancellationToken);
    Task<List<AuditLogs>> GetByOutcomeAsync(AuditLogOutcome outcome, CancellationToken cancellationToken);

    Task AddAsync(AuditLogs auditLog, CancellationToken cancellationToken);
}


