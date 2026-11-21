using EWallet.Infrastructure.Persistence;
using EWallet.Domain.Enums;
using EWallet.Domain.Entities;
using EWallet.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EWallet.Infrastructure.Repositories;

public sealed class WalletMutationRepository(AppDbContext context) : IWalletMutationRepository
{
    public async Task<List<WalletMutations>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.WalletMutations
                .AsNoTracking()
                .OrderByDescending(wm => wm.CreatedAtUtc)
                .ToListAsync(cancellationToken);
    }

    public async Task<List<WalletMutations>> GetByDirectionAsync(WalletMutationDirection direction, CancellationToken cancellationToken)
    {
        return await context.WalletMutations
                .AsNoTracking()
                .Where(wm => wm.Direction == direction)
                .OrderByDescending(wm => wm.CreatedAtUtc)
                .ToListAsync(cancellationToken);
    }

    public async Task<WalletMutations?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await context.WalletMutations.FirstOrDefaultAsync(wm => wm.Id == id, cancellationToken);
    }

    public async Task<List<WalletMutations>> GetByTransactionIdAsync(long transactionId, CancellationToken cancellationToken)
    {
        return await context.WalletMutations
                .AsNoTracking()
                .Where(wm => wm.TransactionId == transactionId)
                .OrderByDescending(wm => wm.CreatedAtUtc)
                .ToListAsync(cancellationToken);
    }

    public async Task<List<WalletMutations>> GetByWalletIdAsync(long walletId, CancellationToken cancellationToken)
    {
        return await context.WalletMutations
                .AsNoTracking()
                .Where(wm => wm.WalletId == walletId)
                .OrderByDescending(wm => wm.CreatedAtUtc)
                .ToListAsync(cancellationToken);
    }


    // METHODS
    public async Task AddAsync(WalletMutations mutation, CancellationToken cancellationToken)
    {
        await context.WalletMutations.AddAsync(mutation, cancellationToken);
    }
}


