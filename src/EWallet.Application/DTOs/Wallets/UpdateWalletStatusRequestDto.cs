using System.ComponentModel.DataAnnotations;
using EWallet.Domain.Enums;

namespace EWallet.Application.Dtos.Wallets;

public sealed record WalletStatusUpdateDto
{
    [Required, EnumDataType(typeof(WalletStatus))]
    public WalletStatus Status { get; init; }
}


