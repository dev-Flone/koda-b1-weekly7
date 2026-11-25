namespace EWallet.Application.DTOs.Admin;

public sealed record AdminUserResponse(
    long Id,
    string UserName,
    string FullName,
    string Email,
    string Status,
    decimal WalletBalance,
    DateTime CreatedAtUtc
);
