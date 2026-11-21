
using EWallet.Application.Dtos.Transactions;
using EWallet.Application.Services.Interfaces;
using EWallet.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TransactionController(ITransactionService service) : ControllerBase
{
    private readonly ITransactionService _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(List<TransactionResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TransactionResponseDto>>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var transactions = await _service.GetAllAsync(cancellationToken);

        return Ok(transactions);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(TransactionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionResponseDto>> GetByIdAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var transaction = await _service.GetByIdAsync(
            id,
            cancellationToken);

        if (transaction is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Transaction not found",
                detail: $"Transaction with ID {id} was not found."
            );
        }

        return Ok(transaction);
    }

    [HttpGet("reference/{referenceNo}")]
    [ProducesResponseType(typeof(TransactionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionResponseDto>> GetByReferenceAsync(
        string referenceNo,
        CancellationToken cancellationToken)
    {
        var transaction = await _service.GetByReferenceNoAsync(
            referenceNo,
            cancellationToken);

        if (transaction is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Transaction not found",
                detail: $"Transaction with reference number {referenceNo} was not found."
            );
        }

        return Ok(transaction);
    }

    [HttpGet("wallet/{walletId:long}")]
    [ProducesResponseType(typeof(List<TransactionResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TransactionResponseDto>>> GetByWalletIdAsync(
        long walletId,
        CancellationToken cancellationToken)
    {
        var transactions = await _service.GetByWalletIdAsync(
            walletId,
            cancellationToken);

        return Ok(transactions);
    }

    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(List<TransactionResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TransactionResponseDto>>> GetByStatusAsync(
        TransactionStatus status,
        CancellationToken cancellationToken)
    {
        var transactions = await _service.GetByStatusAsync(
            status,
            cancellationToken);

        return Ok(transactions);
    }

    [HttpGet("type/{type}")]
    [ProducesResponseType(typeof(List<TransactionResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TransactionResponseDto>>> GetByTypeAsync(
        TransactionType type,
        CancellationToken cancellationToken)
    {
        var transactions = await _service.GetByTypeAsync(
            type,
            cancellationToken);

        return Ok(transactions);
    }

    [HttpGet("history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHistoryAsync(
        [FromQuery] TransactionQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetHistoryAsync(
            query,
            cancellationToken);

        return Ok(result);
    }
}
