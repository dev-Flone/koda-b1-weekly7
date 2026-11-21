using EWallet.Application.Repositories.Interfaces;
using EWallet.Domain.Entities;
using EWallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EWallet.Infrastructure.Repositories;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public async Task<RefreshTokens?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await context.RefreshTokens.FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<List<RefreshTokens>> GetActiveByFamilyIdAsync(Guid familyId, CancellationToken cancellationToken)
    {
        return await context.RefreshTokens
                .Where(token =>
                    token.FamilyId == familyId &&
                    token.RevokedAtUtc == null &&
                    token.ExpiresAtUtc > DateTime.UtcNow).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(RefreshTokens refreshTokens, CancellationToken cancellationToken)
    {
        await context.RefreshTokens.AddAsync(refreshTokens, cancellationToken);
    }

    public void Update(RefreshTokens refreshTokens)
    {
        context.RefreshTokens.Update(refreshTokens);
    }
}
