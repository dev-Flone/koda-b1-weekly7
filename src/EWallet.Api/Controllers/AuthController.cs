using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EWallet.Application.DTOs.Auth;
using EWallet.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public sealed class AuthController(IAuthService service) : ControllerBase
    {
        private readonly IAuthService _service = service;

        // REGISTER
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthResponseDto>> RegisterAsync([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.RegisterAsync(request, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        // LOGIN
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.LoginAsync(request, cancellationToken);

            return Ok(response);
        }

        // REFRESH
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.RefreshAsync(request.RefreshToken, cancellationToken);

            return Ok(response);
        }

        // LOGOUT
        [HttpPost("logout")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> LogoutAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            await _service.LogoutAsync(request.RefreshToken, cancellationToken);

            return NoContent();
        }

        // ME
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.Identity?.Name;

            var email = User.Claims.FirstOrDefault(claim =>
                claim.Type == JwtRegisteredClaimNames.Email ||
                claim.Type == ClaimTypes.Email)?.Value;

            return Ok(new
            {
                UserId = userId,
                UserName = userName,
                Email = email
            });
        }
    }
}
