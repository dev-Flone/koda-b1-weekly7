using System.ComponentModel.DataAnnotations;

namespace EWallet.Application.DTOs.Auth;

public sealed record RegisterRequest
{
    [Required, StringLength(50, MinimumLength = 3)]
    public string UserName { get; init; } = string.Empty;

    [Required, StringLength(150, MinimumLength = 3)]
    public string FullName { get; init; } = string.Empty;

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; init; } = string.Empty;

    [Required, Phone, StringLength(15)]
    public string Phone { get; init; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; init; } = string.Empty;
}
