
using System.Security.Claims;
using EWallet.Application.Dtos.Wallets;
using EWallet.Application.DTOs.Wallets;
using EWallet.Application.Services.Interfaces;
using EWallet.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Api.Controllers;

[ApiController]
[Route("api/wallets")]
public sealed class WalletsController(IWalletService service) : ControllerBase
{
    private readonly IWalletService _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(List<WalletResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WalletResponseDto>>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var wallets = await _service.GetAllAsync(cancellationToken);

        return Ok(wallets);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(WalletResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WalletResponseDto>> GetByIdAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var wallet = await _service.GetByIdAsync(id, cancellationToken);

        if (wallet is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Wallet not found",
                detail: $"Wallet with ID {id} was not found."
            );
        }

        return Ok(wallet);
    }

    [HttpGet("users/{userId:long}")]
    [ProducesResponseType(typeof(WalletResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WalletResponseDto>> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken)
    {
        var wallet = await _service.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (wallet is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Wallet not found",
                detail: $"Wallet for user ID {userId} was not found."
            );
        }

        return Ok(wallet);
    }

    [HttpGet("number/{walletNumber}")]
    [ProducesResponseType(typeof(WalletResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WalletResponseDto>> GetByWalletNumberAsync(
        [FromRoute] string walletNumber,
        CancellationToken cancellationToken)
    {
        var wallet = await _service.GetByWalletNumberAsync(
            walletNumber,
            cancellationToken);

        if (wallet is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Wallet not found",
                detail: $"Wallet with number {walletNumber} was not found."
            );
        }

        return Ok(wallet);
    }

    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(List<WalletResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WalletResponseDto>>> GetByStatusAsync(
        WalletStatus status,
        CancellationToken cancellationToken)
    {
        var wallets = await _service.GetByStatus(
            status,
            cancellationToken);

        return Ok(wallets);
    }

    [HttpPatch("{id:long}/status")]
    [ProducesResponseType(typeof(WalletResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<WalletResponseDto>> UpdateStatusAsync(
        long id,
        [FromBody] WalletStatusUpdateDto request,
        CancellationToken cancellationToken)
    {
        var wallet = await _service.UpdateStatusAsync(
            id,
            request,
            cancellationToken);

        if (wallet is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Wallet not found",
                detail: $"Wallet with ID {id} was not found."
            );
        }

        return Ok(wallet);
    }

    // ME
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(WalletResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WalletResponseDto>> GetMyWalletAsync(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!long.TryParse(userIdClaim, out var userId)) return Unauthorized();

        var wallet = await _service.GetByUserIdAsync(userId, cancellationToken);

        if (wallet is null) return Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Wallet not found",
            detail: "Wallet for the authenticated user was not found."
        );

        return Ok(wallet);
    }

    [HttpGet("recipient/email/{email}")]
    [Authorize]
    [ProducesResponseType(typeof(RecipientWalletResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipientWalletResponse>> GetRecipientByEmailAsync([FromRoute] string email, CancellationToken cancellationToken)
    {
        var recipient = await _service.GetRecipientByEmailAsync(email, cancellationToken);

        if (recipient is null) return Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Recipient not found",
            detail: $"No wallet was found for email {email}"
        );

        return Ok(recipient);
    }

    [HttpGet("recipient/number/{walletNumber}")]
    [Authorize]
    [ProducesResponseType(typeof(RecipientWalletResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipientWalletResponse>> GetRecipientByWalletNumberAsync(
    [FromRoute] string walletNumber,
    CancellationToken cancellationToken)
    {
        var recipient = await _service.GetRecipientByWalletNumberAsync(
            walletNumber,
            cancellationToken
        );

        if (recipient is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Recipient not found",
                detail: $"No wallet was found for number {walletNumber}."
            );
        }

        return Ok(recipient);
    }
}
