
using EWallet.Application.Dtos.TopUps;
using EWallet.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TopUpController(ITopUpService service) : ControllerBase
{
    private readonly ITopUpService _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(List<TopUpResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TopUpResponseDto>>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var topUps = await _service.GetAllAsync(cancellationToken);

        return Ok(topUps);
    }

    [HttpGet("{id:long}", Name = "GetTopUpById")]
    [ProducesResponseType(typeof(TopUpResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TopUpResponseDto>> GetByIdAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var topUp = await _service.GetByIdAsync(id, cancellationToken);

        if (topUp is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Top-up request not found",
                detail: $"Top-up request with ID {id} was not found."
            );
        }

        return Ok(topUp);
    }

    [HttpGet("wallet/{walletId:long}")]
    [ProducesResponseType(typeof(List<TopUpResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TopUpResponseDto>>> GetByWalletIdAsync(
        long walletId,
        CancellationToken cancellationToken)
    {
        var topUps = await _service.GetByWalletIdAsync(
            walletId,
            cancellationToken);

        return Ok(topUps);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TopUpResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TopUpResponseDto>> CreateAsync(
        [FromBody] CreateTopUpRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtRoute(
            "GetTopUpById",
            new { id = created.Id },
            created
        );
    }

    [HttpPost("{id:long}/confirm")]
    [ProducesResponseType(typeof(TopUpResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TopUpResponseDto>> ConfirmAsync(long id, CancellationToken cancellationToken)
    {
        var confirmed = await _service.ConfirmAsync(id, cancellationToken);

        return Ok(confirmed);
    }
}
