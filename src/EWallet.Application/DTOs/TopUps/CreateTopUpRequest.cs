using System.ComponentModel.DataAnnotations;

namespace EWallet.Application.Dtos.TopUps;

public sealed record CreateTopUpRequest
{
    [Required] public long WalletId { get; init; }
    [Range(0, 1)] public int PaymentMethodId { get; init; }
    [Range(typeof(decimal), "0.01", "9999999999999999", ParseLimitsInInvariantCulture = true)] public decimal Amount { get; init; }

}


