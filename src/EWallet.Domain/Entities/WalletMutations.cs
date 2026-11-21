using System.ComponentModel.DataAnnotations;
using EWallet.Domain.Enums;

namespace EWallet.Domain.Entities;

public sealed class WalletMutations(long transactionId, long walletId, decimal amount, decimal balanceBefore, decimal balanceAfter, WalletMutationDirection direction)
{
    public long Id { get; private set; }

    // FK TransactionId
    public long TransactionId { get; private set; } = transactionId;

    // FK WalletId
    public long WalletId { get; private set; } = walletId;

    [Required]
    [StringLength(3)]
    public WalletMutationDirection Direction { get; private set; } = direction;

    [Required]
    public decimal Amount { get; private set; } = amount;

    [Required]
    public decimal BalanceBefore { get; private set; } = balanceBefore;

    [Required]
    public decimal BalanceAfter { get; private set; } = balanceAfter;

    [Required]
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    // Relationship
    public Transactions Transaction { get; private set; } = null!;
    public Wallets Wallets { get; private set; } = null!;
}
