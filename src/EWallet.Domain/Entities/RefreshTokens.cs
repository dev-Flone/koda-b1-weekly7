using System.ComponentModel.DataAnnotations;

namespace EWallet.Domain.Entities;

public sealed class RefreshTokens
{
    private RefreshTokens() { }

    public RefreshTokens(long userId, Guid familyId, string tokenHash, DateTime expiresAtUtc)
    {
        UserId = userId;
        FamilyId = familyId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public Guid FamilyId { get; private set; }

    [Required, MaxLength(64)]
    public string TokenHash { get; private set; } = string.Empty;

    [Required]
    public DateTime ExpiresAtUtc { get; private set; }

    [Required]
    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    [MaxLength(64)]
    public string? ReplacedByTokenHash { get; private set; }

    public Users User { get; private set; } = null!;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsRevoked => RevokedAtUtc.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;
    public void Revoke(string? replacedByTokenHash = null)
    {
        if (IsRevoked) return;

        RevokedAtUtc = DateTime.UtcNow;
        ReplacedByTokenHash = replacedByTokenHash;
    }

}
