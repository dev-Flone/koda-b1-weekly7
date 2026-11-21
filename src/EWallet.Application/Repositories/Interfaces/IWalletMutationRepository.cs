using EWallet.Domain.Enums;
using EWallet.Domain.Entities;

namespace EWallet.Application.Repositories.Interfaces;

public interface IWalletMutationRepository
{
    Task<List<WalletMutations>> GetAllAsync(CancellationToken cancellationToken);
    Task<WalletMutations?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<List<WalletMutations>> GetByWalletIdAsync(long walletId, CancellationToken cancellationToken);
    Task<List<WalletMutations>> GetByTransactionIdAsync(long transactionId, CancellationToken cancellationToken);
    Task<List<WalletMutations>> GetByDirectionAsync(WalletMutationDirection direction, CancellationToken cancellationToken);

    Task AddAsync(WalletMutations mutation, CancellationToken cancellationToken);
}


