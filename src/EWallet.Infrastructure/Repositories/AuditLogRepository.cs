using EWallet.Infrastructure.Persistence;
using EWallet.Domain.Enums;
using EWallet.Domain.Entities;
using EWallet.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EWallet.Infrastructure.Repositories;

public sealed class AuditLogRepository(AppDbContext context) : IAuditLogRepository
{
    public async Task<List<AuditLogs>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.AuditLogs
                .AsNoTracking()
                .OrderByDescending(al => al.CreatedAtUtc)
                .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLogs>> GetByActionAsync(AuditLogAction action, CancellationToken cancellationToken)
    {
        return await context.AuditLogs
                .AsNoTracking()
                .Where(al => al.Action == action)
                .OrderByDescending(al => al.CreatedAtUtc)
                .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLogs>> GetByActorIdAsync(long actorId, CancellationToken cancellationToken)
    {
        return await context.AuditLogs
                .AsNoTracking()
                .Where(al => al.ActorUserId == actorId)
                .OrderByDescending(al => al.CreatedAtUtc)
                .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLogs>> GetByEntityTypeAsync(AuditLogEntityType type, CancellationToken cancellationToken)
    {
        return await context.AuditLogs
                .AsNoTracking()
                .Where(al => al.Type == type)
                .OrderByDescending(al => al.CreatedAtUtc)
                .ToListAsync(cancellationToken);
    }

    public async Task<AuditLogs?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await context.AuditLogs.FirstOrDefaultAsync(al => al.Id == id, cancellationToken);
    }

    public async Task<List<AuditLogs>> GetByOutcomeAsync(AuditLogOutcome outcome, CancellationToken cancellationToken)
    {
        return await context.AuditLogs
                .AsNoTracking()
                .Where(al => al.Outcome == outcome)
                .OrderByDescending(al => al.CreatedAtUtc)
                .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLogs>> GetByTransactionIdAsync(long transactionId, CancellationToken cancellationToken)
    {
        return await context.AuditLogs
                .AsNoTracking()
                .Where(al => al.TransactionId == transactionId)
                .OrderByDescending(al => al.CreatedAtUtc)
                .ToListAsync(cancellationToken);
    }


    // METHODS
    public async Task AddAsync(AuditLogs auditLog, CancellationToken cancellationToken)
    {
        await context.AuditLogs.AddAsync(auditLog, cancellationToken);
    }
}


