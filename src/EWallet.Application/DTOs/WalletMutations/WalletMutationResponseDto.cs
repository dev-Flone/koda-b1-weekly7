using EWallet.Domain.Enums;

namespace EWallet.Application.Dtos.WalletMutations;

public sealed record WalletMutationResponseDto(
    long Id,
    long TransactionId,
    long WalletId,
    WalletMutationDirection Direction,
    decimal Amount,
    decimal BalanceBefore,
    decimal BalanceAfter,
    DateTime CreatedAtUtc
);


