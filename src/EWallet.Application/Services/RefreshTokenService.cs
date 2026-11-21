using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using EWallet.Application.Services.Interfaces;

namespace EWallet.Application.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private const int TokenSizeInBytes = 64;
    private const int TokenLifetimeDays = 7;
    public string GenerateToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(TokenSizeInBytes);

        return Convert.ToBase64String(randomBytes);
    }

    public DateTime GetExpiryDate()
    {
        return DateTime.UtcNow.AddDays(TokenLifetimeDays);
    }

    public string HashToken(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}
