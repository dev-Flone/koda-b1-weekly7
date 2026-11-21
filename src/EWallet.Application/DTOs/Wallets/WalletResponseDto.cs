namespace EWallet.Application.Dtos.Wallets;

public sealed record WalletResponseDto
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public string WalletNumber { get; init; } = string.Empty;
    public decimal Balance { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}


