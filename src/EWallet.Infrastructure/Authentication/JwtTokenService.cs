using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EWallet.Application.Services.Interfaces;
using EWallet.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EWallet.Infrastructure.Authentication;

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public string GenerateToken(Users users, IEnumerable<string> roles)
    {
        var jwtSettings = configuration.GetSection("jwt");

        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT Secret is missing");

        var key = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, users.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, users.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.UniqueName, users.UserName ?? string.Empty),
            new(ClaimTypes.NameIdentifier, users.Id.ToString()),
            new(ClaimTypes.Name, users.UserName ?? string.Empty)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

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
