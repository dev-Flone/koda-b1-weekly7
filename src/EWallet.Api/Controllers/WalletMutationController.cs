using EWallet.Domain.Enums;
using EWallet.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class WalletMutationController(IWalletMutationService walletMutationService) : ControllerBase
{
    // GET: api/WalletMutation
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await walletMutationService.GetAllAsync(cancellationToken);

        return Ok(result);
    }

    // GET: api/WalletMutation/5
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await walletMutationService.GetByIdAsync(id, cancellationToken);

        if (result is null) return NotFound();

        return Ok(result);
    }

    // GET: api/WalletMutation/wallet/10
    [HttpGet("wallet/{walletId:long}")]
    public async Task<IActionResult> GetByWalletId(long walletId, CancellationToken cancellationToken)
    {
        var result = await walletMutationService.GetByWalletIdAsync(walletId, cancellationToken);

        return Ok(result);
    }

    // GET: api/WalletMutation/transaction/15
    [HttpGet("transaction/{transactionId:long}")]
    public async Task<IActionResult> GetByTransactionId(long transactionId, CancellationToken cancellationToken)
    {
        var result = await walletMutationService.GetByTransactionIdAsync(transactionId, cancellationToken);

        return Ok(result);
    }

    // GET: api/WalletMutation/direction/IN
    [HttpGet("direction/{direction}")]
    public async Task<IActionResult> GetByDirection(WalletMutationDirection direction, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(direction))
            return BadRequest(new
            {
                message = "Invalid wallet mutation direction."
            });

        var result = await walletMutationService.GetByDirectionAsync(direction, cancellationToken);

        return Ok(result);
    }
}


