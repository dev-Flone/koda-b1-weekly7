using EWallet.Application.Dtos.Common;
using EWallet.Application.Dtos.Transactions;
using EWallet.Domain.Enums;

namespace EWallet.Application.Services.Interfaces;

public interface ITransactionService
{
    Task<List<TransactionResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<TransactionResponseDto?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<TransactionResponseDto?> GetByReferenceNoAsync(string referenceNo, CancellationToken cancellationToken);
    Task<List<TransactionResponseDto>> GetByWalletIdAsync(long walletId, CancellationToken cancellationToken);
    Task<List<TransactionResponseDto>> GetByStatusAsync(TransactionStatus status, CancellationToken cancellationToken);
    Task<List<TransactionResponseDto>> GetByTypeAsync(TransactionType type, CancellationToken cancellationToken);
    Task<PaginationResponse<TransactionResponseDto>> GetHistoryAsync(TransactionQueryDto query, CancellationToken cancellationToken);
}



