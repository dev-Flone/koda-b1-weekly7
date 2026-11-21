using EWallet.Infrastructure.Persistence;
using EWallet.Domain.Enums;
using EWallet.Application.Repositories.Interfaces;
using EWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EWallet.Infrastructure.Repositories;

public class WalletRepository(AppDbContext context) : IWalletRepository
{
    public async Task<List<Wallets>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Wallets.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Wallets?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await context.Wallets.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<Wallets?> GetByStatus(WalletStatus status, CancellationToken cancellationToken)
    {
        return await context.Wallets.FirstOrDefaultAsync(w => w.Status == status, cancellationToken);
    }

    public async Task<Wallets?> GetByUserId(long userId, CancellationToken cancellationToken)
    {
        return await context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
    }

    public async Task<Wallets?> GetByWalletNumberAsync(string walletNumber, CancellationToken cancellationToken)
    {
        return await context.Wallets.FirstOrDefaultAsync(w => w.WalletNumber == walletNumber, cancellationToken);
    }

    async Task<List<Wallets>> IWalletRepository.GetByStatus(WalletStatus status, CancellationToken cancellationToken)
    {
        return await context.Wallets.Where(w => w.Status == status).ToListAsync(cancellationToken);
    }


    // EXISTS
    public async Task<bool> ExistsByWalletNumber(string walletNumber, CancellationToken cancellationToken)
    {
        return await context.Wallets.AnyAsync(w => w.WalletNumber == walletNumber, cancellationToken);
    }


    // METHODS
    public async Task AddAsync(Wallets wallet, CancellationToken cancellationToken)
    {
        await context.Wallets.AddAsync(wallet, cancellationToken);
    }

    public void Update(Wallets wallet)
    {
        context.Wallets.Update(wallet);
    }

    public void Delete(Wallets wallet)
    {
        context.Remove(wallet);
    }


}


