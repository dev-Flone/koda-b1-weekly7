using EWallet.Domain.Enums;
using EWallet.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PaymentMethodController(
    IPaymentMethodService paymentMethodService
) : ControllerBase
{
    // GET: api/PaymentMethod
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await paymentMethodService.GetAllAsync(
            cancellationToken);

        return Ok(result);
    }

    // GET: api/PaymentMethod/active
    [HttpGet("active")]
    public async Task<IActionResult> GetActive(
        CancellationToken cancellationToken)
    {
        var result = await paymentMethodService.GetActiveAsync(
            cancellationToken);

        return Ok(result);
    }

    // GET: api/PaymentMethod/0
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await paymentMethodService.GetByIdAsync(
            id,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // GET: api/PaymentMethod/code/BANK_TRANSFER
    [HttpGet("code/{code}")]
    public async Task<IActionResult> GetByCode(
        PaymentMethodCode code,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(code))
        {
            return BadRequest(new
            {
                message = "Invalid payment method code."
            });
        }

        var result = await paymentMethodService.GetByCodeAsync(
            code,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}


