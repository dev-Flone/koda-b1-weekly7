using EWallet.Domain.Enums;
using EWallet.Domain.Entities;

namespace EWallet.Application.Repositories.Interfaces;

public interface ITopUpRequestRepository
{
    Task<List<TopUpRequests>> GetAllAsync(CancellationToken cancellationToken);
    Task<TopUpRequests?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<List<TopUpRequests>> GetByWalletIdAsync(long walletId, CancellationToken cancellationToken);
    Task<TopUpRequests?> GetByTransactionIdAsync(long transactionId, CancellationToken cancellationToken);
    Task<List<TopUpRequests>> GetByStatus(TopUpRequestStatus status, CancellationToken cancellationToken);
    Task<TopUpRequests?> GetByProviderReferenceAsync(string providerReference, CancellationToken cancellationToken);
    Task<List<TopUpRequests>> GetExpiredPendingAsync(DateTime utcNow, CancellationToken cancellationToken);

    Task AddAsync(TopUpRequests request, CancellationToken cancellationToken);
    void Update(TopUpRequests request);
}


