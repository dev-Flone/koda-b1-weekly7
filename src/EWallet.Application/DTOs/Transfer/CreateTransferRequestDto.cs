using System.ComponentModel.DataAnnotations;

namespace EWallet.Application.Dtos.Transfer;

public sealed record CreateTransferRequestDto
{
    [Required] public long SenderWalletId { get; init; }
    [Required] public long ReceiverWalletId { get; init; }
    [Range(typeof(decimal), "10000", "999999999999999999999", ParseLimitsInInvariantCulture = true)] public decimal Amount { get; init; }
    [StringLength(250)] public string? Description { get; init; }
}


