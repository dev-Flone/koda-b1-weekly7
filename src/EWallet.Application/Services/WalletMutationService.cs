using EWallet.Application.Dtos.WalletMutations;
using EWallet.Domain.Enums;
using EWallet.Domain.Entities;
using EWallet.Application.Repositories.Interfaces;
using EWallet.Application.Services.Interfaces;

namespace EWallet.Application.Services;

public sealed class WalletMutationService(IWalletMutationRepository repository) : IWalletMutationService
{
    private readonly IWalletMutationRepository _repository = repository;
    public async Task<List<WalletMutationResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var mutation = await _repository.GetAllAsync(cancellationToken);

        return [.. mutation.Select(MapToResponseDto)];
    }

    public async Task<List<WalletMutationResponseDto>> GetByDirectionAsync(WalletMutationDirection direction, CancellationToken cancellationToken)
    {
        var mutation = await _repository.GetByDirectionAsync(direction, cancellationToken);

        return [.. mutation.Select(MapToResponseDto)];
    }

    public async Task<WalletMutationResponseDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var mutation = await _repository.GetByIdAsync(id, cancellationToken);

        return mutation is null ? null : MapToResponseDto(mutation);
    }

    public async Task<List<WalletMutationResponseDto>> GetByTransactionIdAsync(long transactionId, CancellationToken cancellationToken)
    {
        var mutation = await _repository.GetByTransactionIdAsync(transactionId, cancellationToken);

        return [.. mutation.Select(MapToResponseDto)];
    }

    public async Task<List<WalletMutationResponseDto>> GetByWalletIdAsync(long walletId, CancellationToken cancellationToken)
    {
        var mutation = await _repository.GetByWalletIdAsync(walletId, cancellationToken);

        return [.. mutation.Select(MapToResponseDto)];
    }

    // Methods
    private static WalletMutationResponseDto MapToResponseDto(WalletMutations mutation)
    {
        return new WalletMutationResponseDto(
            mutation.Id,
            mutation.TransactionId,
            mutation.WalletId,
            mutation.Direction,
            mutation.Amount,
            mutation.BalanceBefore,
            mutation.BalanceAfter,
            mutation.CreatedAtUtc
        );
    }
}


