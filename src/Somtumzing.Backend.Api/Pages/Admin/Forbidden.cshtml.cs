using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Somtumzing.Backend.Api.Pages.Admin;

[AllowAnonymous]
public sealed class ForbiddenModel : PageModel;
