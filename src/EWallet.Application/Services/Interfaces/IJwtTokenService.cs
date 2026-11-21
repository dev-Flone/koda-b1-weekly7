using EWallet.Domain.Entities;

namespace EWallet.Application.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(Users User);
}
