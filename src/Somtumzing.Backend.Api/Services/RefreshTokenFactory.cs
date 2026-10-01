using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Api.Services;

public static class RefreshTokenFactory
{
    public static (string PlainText, RefreshToken Entity) Create(Guid userId, DateTimeOffset now)
    {
        var plainText = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));
        var tokenHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(plainText)));
        var entity = new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(30)
        };

        return (plainText, entity);
    }

    public static string Hash(string plainText) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(plainText)));
}
