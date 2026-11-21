namespace EWallet.Application.Dtos.Transactions;

public sealed record TransactionResponseDto
{
    public long Id { get; init; }
    public string ReferenceNo { get; init; } = string.Empty;
    public long? SenderWalletId { get; init; }
    public long ReceiverWalletId { get; init; }
    public string Type { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
}


