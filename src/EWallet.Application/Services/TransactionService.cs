using EWallet.Application.Dtos.Common;
using EWallet.Application.Dtos.Transactions;
using EWallet.Domain.Enums;
using EWallet.Domain.Entities;
using EWallet.Application.Repositories.Interfaces;
using EWallet.Application.Services.Interfaces;

namespace EWallet.Application.Services;

public class TransactionService(ITransactionRepository repository) : ITransactionService
{
    private readonly ITransactionRepository _repository = repository;
    public async Task<List<TransactionResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var transaction = await _repository.GetAllAsync(cancellationToken);

        return [.. transaction.Select(MapToResponse)];
    }

    public async Task<TransactionResponseDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var transaction = await _repository.GetByIdAsync(id, cancellationToken);

        return transaction is null ? null : MapToResponse(transaction);
    }

    public async Task<TransactionResponseDto?> GetByReferenceNoAsync(string referenceNo, CancellationToken cancellationToken)
    {
        var transaction = await _repository.GetByReferenceNoAsync(referenceNo, cancellationToken);

        return transaction is null ? null : MapToResponse(transaction);
    }

    public async Task<List<TransactionResponseDto>> GetByStatusAsync(TransactionStatus status, CancellationToken cancellationToken)
    {
        var transaction = await _repository.GetByStatusAsync(status, cancellationToken);

        return [.. transaction.Select(MapToResponse)];
    }

    public async Task<List<TransactionResponseDto>> GetByTypeAsync(TransactionType type, CancellationToken cancellationToken)
    {
        var transaction = await _repository.GetByTypeAsync(type, cancellationToken);

        return [.. transaction.Select(MapToResponse)];
    }

    public async Task<List<TransactionResponseDto>> GetByWalletIdAsync(long walletId, CancellationToken cancellationToken)
    {
        var transaction = await _repository.GetByWalletId(walletId, cancellationToken);

        return [.. transaction.Select(MapToResponse)];
    }

    public async Task<PaginationResponse<TransactionResponseDto>> GetHistoryAsync(TransactionQueryDto query, CancellationToken cancellationToken)
    {
        return await _repository.GetHistoryAsync(query, cancellationToken);
    }

    // Methods
    private static TransactionResponseDto MapToResponse(
        Transactions transaction)
    {
        return new TransactionResponseDto
        {
            Id = transaction.Id,
            ReferenceNo = transaction.ReferenceNo,
            SenderWalletId = transaction.SenderWalletId,
            ReceiverWalletId = transaction.ReceiverWalletId,
            Type = transaction.Type.ToString(),
            Amount = transaction.Amount,
            Status = transaction.Status.ToString(),
            Description = transaction.Description,
            CreatedAtUtc = transaction.CreatedAtUtc,
            CompletedAtUtc = transaction.CompletedAtUtc
        };
    }
}


