using System.ComponentModel.DataAnnotations;
using EWallet.Domain.Enums;

namespace EWallet.Domain.Entities;

public sealed class TopUpRequests(long transactionId, long walletId, int paymentMethodId, decimal amount, TopUpRequestStatus status = TopUpRequestStatus.PENDING, string? providerReference = null, DateTime? expiresAtUtc = null, DateTime? paidAtUtc = null)
{
    public long Id { get; private set; }

    // FK TransactionId
    public long TransactionId { get; private set; } = transactionId;

    // FK WalletId
    public long WalletId { get; private set; } = walletId;

    // FK PaymentMethodId
    public int PaymentMethodId { get; private set; } = paymentMethodId;

    [Required]
    public decimal Amount { get; private set; } = amount;

    [Required]
    [StringLength(20)]
    public TopUpRequestStatus Status { get; private set; } = status;

    [StringLength(100)]
    public string? ProviderReference { get; private set; } = providerReference;

    [Required]
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? ExpiresAtUtc { get; private set; } = expiresAtUtc;
    public DateTime? PaidAtUtc { get; private set; } = paidAtUtc;

    // Relationship
    public Transactions Transaction { get; private set; } = null!;
    public Wallets Wallet { get; private set; } = null!;
    public PaymentMethods PaymentMethod { get; private set; } = null!;

    // METHODS
    public void Confirmation()
    {
        if (Status != TopUpRequestStatus.PENDING) throw new InvalidOperationException("Only pending top-up requests can be marked as paid.");

        Status = TopUpRequestStatus.PAID;
        PaidAtUtc = DateTime.UtcNow;
    }
}
