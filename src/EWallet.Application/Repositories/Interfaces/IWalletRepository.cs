using EWallet.Domain.Enums;
using EWallet.Domain.Entities;

namespace EWallet.Application.Repositories.Interfaces;

public interface IWalletRepository
{
    Task<List<Wallets>> GetAllAsync(CancellationToken cancellationToken);
    Task<Wallets?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<Wallets?> GetByUserId(long userId, CancellationToken cancellationToken);
    Task<Wallets?> GetByWalletNumberAsync(string walletNumber, CancellationToken cancellationToken);
    Task<List<Wallets>> GetByStatus(WalletStatus status, CancellationToken cancellationToken);

    Task<bool> ExistsByWalletNumber(string walletNumber, CancellationToken cancellationToken);

    Task AddAsync(Wallets wallet, CancellationToken cancellationToken);
    void Update(Wallets wallet);
    void Delete(Wallets wallet);
}


