using Microsoft.AspNetCore.Identity;

namespace Somtumzing.Backend.Api.Models;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<RefreshToken> RefreshTokens { get; } = new List<RefreshToken>();
}
