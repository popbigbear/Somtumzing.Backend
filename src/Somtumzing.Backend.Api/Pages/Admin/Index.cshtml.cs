using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Api.Pages.Admin;

public sealed class IndexModel(SignInManager<ApplicationUser> signInManager) : PageModel
{
    public async Task<IActionResult> OnPostSignOutAsync()
    {
        await signInManager.SignOutAsync();
        return RedirectToPage("/Admin/Login");
    }
}
