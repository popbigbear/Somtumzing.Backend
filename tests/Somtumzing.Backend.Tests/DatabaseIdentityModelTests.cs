using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Tests;

public sealed class DatabaseIdentityModelTests
{
    [Fact]
    public void UserRoleUsesIdentityCompositeKeyAndRetainsSurrogateKey()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseNpgsql("Host=localhost;Database=somtumzing")
            .Options;

        using var database = new GameDbContext(options);
        var entity = database.Model.FindEntityType(typeof(ApplicationUserRole));

        Assert.NotNull(entity);
        Assert.Equal(
            new[] { nameof(ApplicationUserRole.UserId), nameof(ApplicationUserRole.RoleId) },
            entity.FindPrimaryKey()!.Properties.Select(property => property.Name));
        Assert.Contains(
            entity.GetKeys(),
            key => !key.IsPrimaryKey()
                && key.Properties.Select(property => property.Name)
                    .SequenceEqual([nameof(ApplicationUserRole.PK)]));
    }
}
