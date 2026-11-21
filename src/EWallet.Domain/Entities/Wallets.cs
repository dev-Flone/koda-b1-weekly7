
using System.ComponentModel.DataAnnotations;
using EWallet.Domain.Enums;

namespace EWallet.Domain.Entities;

public sealed class Wallets(
    long userId,
    string walletNumber,
    decimal balance)
{
    public long Id { get; private set; }

    public long UserId { get; private set; } = userId;

    [Required, MaxLength(30)]
    public string WalletNumber { get; private set; } = walletNumber;

    [Required]
    public decimal Balance { get; private set; } = balance;

    [Required, StringLength(20)]
    public WalletStatus Status { get; private set; } = WalletStatus.ACTIVE;

    [Required]
    public byte[] RowVersion { get; private set; } = null!;

    [Required]
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; private set; }

    public Users Users { get; private set; } = null!;

    public ICollection<Transactions> SentTransactions { get; private set; } = [];
    public ICollection<Transactions> ReceivedTransactions { get; private set; } = [];
    public ICollection<WalletMutations> WalletMutations { get; private set; } = [];
    public ICollection<TopUpRequests> TopUpRequests { get; private set; } = [];

    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        if (Status != WalletStatus.ACTIVE)
            throw new InvalidOperationException(
                "Only active wallets can receive funds.");

        Balance += amount;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Debit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        if (Status != WalletStatus.ACTIVE)
            throw new InvalidOperationException(
                "Only active wallets can send funds.");

        if (Balance < amount)
            throw new InvalidOperationException("Insufficient balance.");

        Balance -= amount;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateStatus(WalletStatus status)
    {
        if (Status == WalletStatus.CLOSED)
            throw new InvalidOperationException(
                "A closed wallet cannot change its status.");

        if (Status == status)
            return;

        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
