using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EWallet.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace EWallet.Domain.Entities;

public sealed class Users : IdentityUser<long>
{
    private Users() { }
    public Users(
        string userName,
        string fullName,
        string email,
        string phone
    )
    {
        UserName = userName;
        NormalizedUserName = userName.ToUpperInvariant();

        FullName = fullName;

        Email = email;
        NormalizedEmail = email.ToUpperInvariant();

        Phone = phone;

        Status = UserStatus.ACTIVE;
        CreatedAtUtc = DateTime.UtcNow;
        SecurityStamp = Guid.NewGuid().ToString();
    }

    [Required, MaxLength(100)]
    public string FullName { get; private set; } = string.Empty;

    [Required]
    public UserStatus Status { get; private set; } = UserStatus.ACTIVE;

    [Required]
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; private set; }

    [NotMapped]
    public string Phone
    {
        get => PhoneNumber ?? string.Empty;
        private set => PhoneNumber = value;
    }

    public Wallets Wallets { get; private set; } = null!;
    public ICollection<AuditLogs> AuditLogs { get; private set; } = [];
    public ICollection<RefreshTokens> RefreshTokens { get; private set; } = [];


    // Method
    public void Suspend()
    {
        if (Status == UserStatus.SUSPENDED)
            return;

        Status = UserStatus.SUSPENDED;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateProfile(
        string username,
        string fullname,
        string email,
        string phone)
    {
        UserName = username;
        NormalizedUserName = username.ToUpperInvariant();

        FullName = fullname;

        Email = email;
        NormalizedEmail = email.ToUpperInvariant();

        PhoneNumber = phone;

        UpdatedAtUtc = DateTime.UtcNow;
    }
}
