using EWallet.Domain.Enums;
using EWallet.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuditLogController(IAuditLogService auditLogService) : ControllerBase
{
    // GET: api/AuditLog
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await auditLogService.GetAllAsync(cancellationToken);

        return Ok(result);
    }

    // GET: api/AuditLog/1
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await auditLogService.GetByIdAsync(id, cancellationToken);

        if (result is null) return NotFound();

        return Ok(result);
    }

    // GET: api/AuditLog/user/1
    [HttpGet("user/{actorUserId:long}")]
    public async Task<IActionResult> GetByActorUserId(long actorUserId, CancellationToken cancellationToken)
    {
        var result = await auditLogService.GetByActorUserIdAsync(actorUserId, cancellationToken);

        return Ok(result);
    }

    // GET: api/AuditLog/transaction/1
    [HttpGet("transaction/{transactionId:long}")]
    public async Task<IActionResult> GetByTransactionId(long transactionId, CancellationToken cancellationToken)
    {
        var result = await auditLogService.GetByTransactionIdAsync(transactionId, cancellationToken);

        return Ok(result);
    }

    // GET: api/AuditLog/action/TRANSFER_CREATED
    [HttpGet("action/{actionName}")]
    public async Task<IActionResult> GetByAction(AuditLogAction actionName, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(actionName))
        {
            return BadRequest(new
            {
                message = "Invalid audit log action"
            });
        }
        var result = await auditLogService.GetByActionAsync(actionName, cancellationToken);

        return Ok(result);
    }

    // GET: api/AuditLog/entity-type/WALLET
    [HttpGet("entity-type/{type}")]
    public async Task<IActionResult> GetByEntityType(AuditLogEntityType type, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(type))
        {
            return BadRequest(new
            {
                message = "Invalid audit log entity type."
            });
        }

        var result = await auditLogService.GetByEntityTypeAsync(type, cancellationToken);

        return Ok(result);
    }

    // GET: api/AuditLog/outcome/SUCCESS
    [HttpGet("outcome/{outcome}")]
    public async Task<IActionResult> GetByOutcome(AuditLogOutcome outcome, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(outcome))
        {
            return BadRequest(new
            {
                message = "Invalid audit log outcome."
            });
        }

        var result = await auditLogService.GetByOutcomeAsync(outcome, cancellationToken);

        return Ok(result);
    }
}


