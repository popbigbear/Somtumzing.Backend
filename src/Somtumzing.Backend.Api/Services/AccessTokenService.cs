using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Api.Services;

public sealed class AccessTokenService(byte[] signingKey, string issuer, string audience)
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(15);

    public string CreatePlayerToken(ApplicationUser user, DateTimeOffset now)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString("D")),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("D")),
            new Claim("name", user.UserName ?? user.Id.ToString("D")),
            new Claim(
                "sst",
                user.SecurityStamp
                    ?? throw new InvalidOperationException("Player security stamp is not initialized.")),
            new Claim("role", "Player")
        };
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(signingKey),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            now.UtcDateTime,
            now.Add(TokenLifetime).UtcDateTime,
            credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
