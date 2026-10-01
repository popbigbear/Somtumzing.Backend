using Microsoft.AspNetCore.Mvc;

namespace Somtumzing.Backend.Api.Controllers;

[ApiController]
[Route("api/v1/system")]
public sealed class SystemController : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus() =>
        Ok(new { status = "ok", service = "somtumzing-backend-api" });
}
