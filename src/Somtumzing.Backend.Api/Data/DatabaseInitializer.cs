using Microsoft.AspNetCore.Identity;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Api.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration)
    {
        var email = configuration["BackOffice:AdminEmail"];
        var password = configuration["BackOffice:AdminPassword"];
        if (string.IsNullOrWhiteSpace(email) != string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "BackOffice:AdminEmail and BackOffice:AdminPassword must be configured together.");
        }

        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        foreach (var role in new[] { "Admin", "Player" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new ApplicationRole { Name = role });
                EnsureSucceeded(result, $"creating the {role} role");
            }
        }

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await userManager.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                CreatedAtUtc = DateTimeOffset.UtcNow
            };
            var createResult = await userManager.CreateAsync(admin, password);
            EnsureSucceeded(createResult, "creating the configured back-office administrator");
        }
        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            var roleResult = await userManager.AddToRoleAsync(admin, "Admin");
            EnsureSucceeded(roleResult, "granting the configured back-office administrator role");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string operation)
    {
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(error => error.Code));
            throw new InvalidOperationException($"Identity failed while {operation}: {errors}");
        }
    }
}
