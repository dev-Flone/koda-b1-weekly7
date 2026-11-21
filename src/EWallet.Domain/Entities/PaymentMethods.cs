using System.ComponentModel.DataAnnotations;
using EWallet.Domain.Enums;

namespace EWallet.Domain.Entities;

public sealed class PaymentMethods(int id, PaymentMethodCode code, string name, string? providerName, bool isActive)
{
    public int Id { get; private set; } = id;

    [StringLength(30)]
    public PaymentMethodCode Code { get; private set; } = code;

    [Required]
    [StringLength(100)]
    public string Name { get; private set; } = name;

    [StringLength(100)]
    public string? ProviderName { get; private set; } = providerName;

    [Required]
    public bool IsActive { get; private set; } = isActive;

    [Required]
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow.AddHours(7);

    // Relationship
    public ICollection<TopUpRequests> TopUpRequests { get; private set; } = [];
}
