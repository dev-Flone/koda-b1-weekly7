using System.ComponentModel.DataAnnotations;

namespace EWallet.Application.DTOs.Auth;

public sealed record LoginRequest
{
    [Required]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
