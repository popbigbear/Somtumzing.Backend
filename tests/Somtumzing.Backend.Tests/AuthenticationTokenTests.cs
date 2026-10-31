using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using Somtumzing.Backend.Api.Models;
using Somtumzing.Backend.Api.Services;

namespace Somtumzing.Backend.Tests;

public sealed class AuthenticationTokenTests
{
    [Fact]
    public void AccessTokenContainsPlayerIdentityAndExpiresAfterFifteenMinutes()
    {
        var key = RandomNumberGenerator.GetBytes(32);
        var user = new ApplicationUser { Id = Guid.NewGuid(), SecurityStamp = "test-security-stamp" };
        var now = DateTimeOffset.UtcNow;
        var service = new AccessTokenService(key, "somtumzing-test", "somtumzing-game-test");

        var token = service.CreatePlayerToken(user, now);
        var parts = token.Split('.');
        Assert.Equal(3, parts.Length);
        var signingInput = Encoding.ASCII.GetBytes($"{parts[0]}.{parts[1]}");
        var signature = DecodeBase64Url(parts[2]);
        var expectedSignature = HMACSHA256.HashData(key, signingInput);
        using var payload = JsonDocument.Parse(DecodeBase64Url(parts[1]));

        Assert.Equal(expectedSignature, signature);
        Assert.Equal("somtumzing-test", payload.RootElement.GetProperty("iss").GetString());
        Assert.Equal("somtumzing-game-test", payload.RootElement.GetProperty("aud").GetString());
        Assert.Equal(user.Id.ToString("D"), payload.RootElement.GetProperty("sub").GetString());
        Assert.Equal("Player", payload.RootElement.GetProperty("role").GetString());
        Assert.Equal(user.SecurityStamp, payload.RootElement.GetProperty("sst").GetString());
        Assert.Equal(
            now.AddMinutes(15).ToUnixTimeSeconds(),
            payload.RootElement.GetProperty("exp").GetInt64());
    }

    [Fact]
    public void RefreshTokenStoresOnlyHashAndExpiresAfterThirtyDays()
    {
        var now = DateTimeOffset.UtcNow;
        var userId = Guid.NewGuid();

        var (plainText, entity) = RefreshTokenFactory.Create(userId, now);

        Assert.Equal(userId, entity.UserId);
        Assert.Equal(64, entity.TokenHash.Length);
        Assert.NotEqual(plainText, entity.TokenHash);
        Assert.Equal(RefreshTokenFactory.Hash(plainText), entity.TokenHash);
        Assert.Equal(now.AddDays(30), entity.ExpiresAtUtc);
    }

    [Fact]
    public void RefreshTokenFactoryGeneratesDistinctCredentials()
    {
        var first = RefreshTokenFactory.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);
        var second = RefreshTokenFactory.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);

        Assert.NotEqual(first.PlainText, second.PlainText);
        Assert.NotEqual(first.Entity.TokenHash, second.Entity.TokenHash);
    }

    private static byte[] DecodeBase64Url(string value)
    {
        var base64 = value.Replace('-', '+').Replace('_', '/');
        base64 = base64.PadRight(base64.Length + ((4 - base64.Length % 4) % 4), '=');
        return Convert.FromBase64String(base64);
    }
}
