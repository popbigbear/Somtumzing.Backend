using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Api.Pages.Admin;

[AllowAnonymous]
public sealed class LoginModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    ILogger<LoginModel> logger) : PageModel
{
    private static readonly EventId AdminLoginSucceededEvent = new(1101, "AdminLoginSucceeded");
    private static readonly EventId AdminLoginFailedEvent = new(1102, "AdminLoginFailed");

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty]
    public string? ReturnUrl { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await signInManager.PasswordSignInAsync(
            Input.Email,
            Input.Password,
            Input.RememberMe,
            lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            logger.LogWarning(
                AdminLoginFailedEvent,
                "Back-office sign-in failed. {EventType} {Outcome}",
                "AdminLogin",
                result.IsLockedOut ? "LockedOut" : "InvalidCredentials");
            ModelState.AddModelError(string.Empty, "Unable to sign in with those credentials.");
            return Page();
        }

        var administrator = await userManager.FindByEmailAsync(Input.Email);
        if (administrator is null)
        {
            throw new InvalidOperationException("Signed-in back-office user could not be resolved.");
        }

        logger.LogInformation(
            AdminLoginSucceededEvent,
            "Back-office sign-in succeeded. {EventType} {AdminId} {Outcome}",
            "AdminLogin",
            administrator.Id,
            "Succeeded");

        var destination = !string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl)
            ? ReturnUrl
            : Url.Page("/Admin/Index");
        return LocalRedirect(destination ?? "/Admin");
    }

    public sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
