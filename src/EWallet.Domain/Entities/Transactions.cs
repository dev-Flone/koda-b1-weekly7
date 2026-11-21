using System.ComponentModel.DataAnnotations;
using EWallet.Domain.Enums;

namespace EWallet.Domain.Entities;

public sealed class Transactions(
    string referenceNo, long receiverWalletId, decimal amount, TransactionType type, long? senderWalletId = null, TransactionStatus status = TransactionStatus.PENDING, string? description = null, DateTime? completedAtUtc = null)
{
    public long Id { get; private set; }

    [Required]
    [StringLength(50)]
    public string ReferenceNo { get; private set; } = referenceNo;
    public long? SenderWalletId { get; private set; } = senderWalletId;

    [Required]
    public long ReceiverWalletId { get; private set; } = receiverWalletId;

    [Required]
    [StringLength(20)]
    public TransactionType Type { get; private set; } = type;

    [Required]
    public decimal Amount { get; private set; } = amount;

    [Required]
    [StringLength(20)]
    public TransactionStatus Status { get; private set; } = status;

    [MaxLength(255)]
    public string? Description { get; private set; } = description;

    [Required]
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; private set; } = completedAtUtc;

    // Relationship
    public Wallets? SenderWallet { get; private set; }
    public Wallets ReceiverWallet { get; private set; } = null!;
    public TopUpRequests? TopUpRequest { get; private set; }
    public ICollection<WalletMutations> WalletMutations { get; private set; } = [];
    public ICollection<AuditLogs> AuditLogs { get; private set; } = [];

    // METHODS
    public void Confirmation()
    {
        if (Type != TransactionType.TOPUP) throw new InvalidOperationException("Only top-up transactions can be marked as successfull.");

        if (Status != TransactionStatus.PENDING) throw new InvalidOperationException("Only pending transactions can be marked as successfull.");

        Status = TransactionStatus.SUCCESS;
        CompletedAtUtc = DateTime.UtcNow;
    }
}
