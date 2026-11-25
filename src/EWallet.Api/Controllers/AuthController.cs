using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EWallet.Application.DTOs.Admin;
using EWallet.Application.DTOs.Auth;
using EWallet.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EWallet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class AuthController(IAuthService service, IWebHostEnvironment environment) : ControllerBase
    {
        private readonly IAuthService _service = service;
        private readonly IWebHostEnvironment _environment = environment;
        private const string RefreshTokenCookie = "refreshToken";

        // REGISTER
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthResponseDto>> RegisterAsync([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.RegisterAsync(request, cancellationToken);

            SetRefreshTokenCookie(response);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        // LOGIN
        [AllowAnonymous]
        [HttpPost("login")]
        [EnableRateLimiting("AuthPolicy")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.LoginAsync(request, cancellationToken);

            SetRefreshTokenCookie(response);

            return Ok(response);
        }

        // REFRESH
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> RefreshAsync(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies[RefreshTokenCookie];

            if (string.IsNullOrWhiteSpace(refreshToken)) return Unauthorized();

            var response = await _service.RefreshAsync(refreshToken, cancellationToken);

            SetRefreshTokenCookie(response);

            return Ok(response);
        }

        // LOGOUT
        [HttpPost("logout")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies[RefreshTokenCookie];

            if (!string.IsNullOrWhiteSpace(refreshToken)) await _service.LogoutAsync(refreshToken, cancellationToken);

            DeleteRefreshTokenCookie();

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

            var role = User.FindFirstValue(ClaimTypes.Role);

            return Ok(new
            {
                UserId = userId,
                UserName = userName,
                Email = email,
                Role = role
            });
        }

        // Helper Methods
        private void SetRefreshTokenCookie(AuthResponseDto response)
        {
            Response.Cookies.Append(RefreshTokenCookie, response.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = !_environment.IsDevelopment(),
                SameSite = SameSiteMode.Lax,
                Expires = response.RefreshTokenExpiresAtUtc,
                Path = "/"
            });
        }

        private void DeleteRefreshTokenCookie()
        {
            Response.Cookies.Delete(RefreshTokenCookie, new CookieOptions
            {
                HttpOnly = true,
                Secure = !_environment.IsDevelopment(),
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });
        }
    }
}
