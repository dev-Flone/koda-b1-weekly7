
using EWallet.Application.Dtos.Transfer;
using EWallet.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TransferController(ITransferService service) : ControllerBase
{
    private readonly ITransferService _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(List<TransferResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TransferResponseDto>>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var transfers = await _service.GetAllAsync(cancellationToken);

        return Ok(transfers);
    }

    [HttpGet("{id:long}", Name = "GetTransferById")]
    [ProducesResponseType(typeof(TransferResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransferResponseDto>> GetByIdAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var transfer = await _service.GetByIdAsync(id, cancellationToken);

        if (transfer is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Transfer not found",
                detail: $"Transfer with ID {id} was not found."
            );
        }

        return Ok(transfer);
    }

    [HttpGet("wallet/{walletId:long}")]
    [ProducesResponseType(typeof(List<TransferResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TransferResponseDto>>> GetByWalletIdAsync(
        long walletId,
        CancellationToken cancellationToken)
    {
        var transfers = await _service.GetByWalletIdAsync(
            walletId,
            cancellationToken);

        return Ok(transfers);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TransferResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TransferResponseDto>> CreateAsync(
        [FromBody] CreateTransferRequestDto request,
        CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtRoute(
            "GetTransferById",
            new { id = created.TransactionId },
            created
        );
    }
}
