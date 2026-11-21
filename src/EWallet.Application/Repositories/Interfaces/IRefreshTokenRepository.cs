using EWallet.Domain.Entities;

namespace EWallet.Application.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshTokens?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);
    Task<List<RefreshTokens>> GetActiveByFamilyIdAsync(Guid familyId, CancellationToken cancellationToken);
    Task AddAsync(RefreshTokens refreshTokens, CancellationToken cancellationToken);
    void Update(RefreshTokens refreshTokens);
}
