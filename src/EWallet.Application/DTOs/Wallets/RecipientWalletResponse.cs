namespace EWallet.Application.DTOs.Wallets;

public sealed record RecipientWalletResponse
{
    public long WalletId { get; init; }
    public string WalletNumber { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}
