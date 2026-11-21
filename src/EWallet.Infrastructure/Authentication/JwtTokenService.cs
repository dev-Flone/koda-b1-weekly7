using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EWallet.Application.Services.Interfaces;
using EWallet.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EWallet.Infrastructure.Authentication;

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public string GenerateToken(Users User)
    {
        var jwtSettings = configuration.GetSection("jwt");

        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is missing");

        var key = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, User.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, User.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.UniqueName, User.UserName ?? string.Empty),
            new(ClaimTypes.NameIdentifier, User.Id.ToString()),
            new(ClaimTypes.Name, User.UserName ?? string.Empty)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
