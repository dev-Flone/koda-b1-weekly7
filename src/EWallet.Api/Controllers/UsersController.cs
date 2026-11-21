
using EWallet.Application.Dtos.Users;
using EWallet.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController(IUserService service) : ControllerBase
{
    private readonly IUserService _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(List<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserResponseDto>>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var users = await _service.GetAllAsync(cancellationToken);

        return Ok(users);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDto>> GetByIdAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var user = await _service.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found",
                detail: $"User with ID {id} was not found."
            );
        }

        return Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponseDto>> CreateAsync(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = created.Id },
            created
        );
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponseDto>> UpdateAsync(
        [FromRoute] long id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _service.UpdateAsync(
            id,
            request,
            cancellationToken);

        if (updated is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found",
                detail: $"User with ID {id} was not found."
            );
        }

        return Ok(updated);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found",
                detail: $"User with ID {id} was not found."
            );
        }

        return NoContent();
    }
}
