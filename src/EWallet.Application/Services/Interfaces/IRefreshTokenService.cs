namespace EWallet.Application.Services.Interfaces;

public interface IRefreshTokenService
{
    string GenerateToken();
    string HashToken(string token);
    DateTime GetExpiryDate();
}
