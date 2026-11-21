namespace EWallet.Application.Dtos.Transfer;

public sealed record TransferResponseDto
{
    public long TransactionId { get; init; }
    public string ReferenceNo { get; init; } = string.Empty;
    public long SenderWalletId { get; init; }
    public long ReceiverWalletId { get; init; }
    public decimal Amount { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
}


