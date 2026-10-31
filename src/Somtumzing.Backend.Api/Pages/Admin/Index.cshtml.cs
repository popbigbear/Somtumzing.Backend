using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Somtumzing.Backend.Api.Services;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Api.Pages.Admin;

public sealed class IndexModel(
    SignInManager<ApplicationUser> signInManager,
    IAuditLogWriter auditLogWriter) : PageModel
{
    public async Task<IActionResult> OnPostSignOutAsync()
    {
        if (Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminId))
        {
            await auditLogWriter.WriteAsync(
                "AdminLogout",
                "Information",
                "Succeeded",
                HttpContext.RequestAborted,
                actorUserId: adminId,
                actorUsername: User.Identity?.Name,
                targetType: "BackOffice",
                targetId: adminId.ToString("D"));
        }

        await signInManager.SignOutAsync();
        return RedirectToPage("/Admin/Login");
    }
}
