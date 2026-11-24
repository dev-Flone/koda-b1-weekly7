using System.Text.Json.Serialization;

namespace EWallet.Application.DTOs.Auth;

public sealed record AuthResponseDto(
    string AccessToken,
    DateTime ExpiresAtUtc,

    [property: JsonIgnore]
    string RefreshToken,

    [property: JsonIgnore]
    DateTime RefreshTokenExpiresAtUtc,

    long UserId,
    string UserName,
    string FullName,
    string Email
);