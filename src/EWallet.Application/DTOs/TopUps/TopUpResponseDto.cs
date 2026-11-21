namespace EWallet.Application.Dtos.TopUps;

public sealed record TopUpResponseDto
{
    public long Id { get; init; }
    public long TransactionId { get; init; }
    public long WalletId { get; init; }
    public int PaymentMethodId { get; init; }
    public decimal Amount { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? ProviderReference { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? ExpiresAtUtc { get; init; }
    public DateTime? PaidAtUtc { get; init; }
}


