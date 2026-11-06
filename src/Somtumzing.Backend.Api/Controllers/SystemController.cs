using Microsoft.AspNetCore.Mvc;

namespace Somtumzing.Backend.Api.Controllers;

[ApiController]
[Route("api/v1/system")]
public sealed class SystemController(ILogger<SystemController> logger) : ControllerBase
{
    private static readonly EventId SystemStatusCheckedEvent = new(4001, "SystemStatusChecked");

    /// <summary>
    /// ตรวจสอบสถานะการทำงานของระบบ (Health Check)
    /// GET /api/v1/system/status
    /// Security Level: Public (ไม่ต้องใช้ Token)
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(typeof(SystemStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetStatus()
    {
        try
        {
            var response = new SystemStatusResponse(
                Status: "ok",
                Service: "somtumzing-backend-api",
                ServerTimeUtc: DateTimeOffset.UtcNow);

            logger.LogInformation(SystemStatusCheckedEvent, "System status checked successfully.");

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while checking system status.");
            
            // ส่งคืน Error ตามมาตรฐาน RFC 7807 (สถานะ 500 Backend Error)
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Failed to retrieve system status.",
                Instance = HttpContext.Request.Path
            });
        }
    }

    /// <summary>
    /// โครงสร้างข้อมูลสถานะระบบส่งกลับให้ Client
    /// </summary>
    public sealed record SystemStatusResponse(
        string Status,
        string Service,
        DateTimeOffset ServerTimeUtc);
}