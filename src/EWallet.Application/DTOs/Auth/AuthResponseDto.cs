namespace EWallet.Application.DTOs.Auth;

public sealed record AuthResponseDto(
    string AccessToken,
    DateTime ExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc,
    long UserId,
    string UserName,
    string FullName,
    string Email
);