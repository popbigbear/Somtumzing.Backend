using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Somtumzing.Backend.Api.Data;

public sealed class GameDbContextFactory : IDesignTimeDbContextFactory<GameDbContext>
{
    public GameDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__GameDatabase")
            ?? throw new InvalidOperationException(
                "Set ConnectionStrings__GameDatabase before creating or applying migrations.");
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new GameDbContext(options);
    }
}
