using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Api.Controllers;

/// <summary>
/// Controller สำหรับจัดการ Customer Table Configuration
/// </summary>
[ApiController]
[Authorize(Roles = "Player")]
[Route("api/v1/customer-tables")]
public sealed class CustomerTableController(
    GameDbContext database,
    ILogger<CustomerTableController> logger) : ControllerBase
{
    private static readonly EventId CustomerTablesLoadedEvent = new(2101, "CustomerTablesLoaded");
    private static readonly EventId CustomerTableLoadedEvent = new(2102, "CustomerTableLoaded");

    /// <summary>
    /// ดึงรายการโต๊ะลูกค้าทั้งหมด
    /// GET /api/v1/customer-tables
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CustomerTablesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerTablesResponse>> GetCustomerTables(CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(subject) || !Guid.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        var user = await database.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null) return Unauthorized();
        if (user.IsSuspended) return ForbiddenProblem();

        var tables = await database.CustomerTables
            .AsNoTracking()
            .Where(t => t.Status == "ACTIVE" && t.IsActive)
            .OrderBy(t => t.TableNumber)
            .ToListAsync(cancellationToken);

        var items = tables.Select(t => new CustomerTableItem(
            Id: t.PK,
            TableNumber: t.TableNumber,
            Capacity: t.Capacity,
            InitialStatus: t.InitialStatus,
            AssignedCustomerType: t.AssignedCustomerType,
            AssignedCustomerId: t.AssignedCustomerId,
            PositionX: t.PositionX,
            PositionY: t.PositionY,
            Zone: t.Zone,
            IsActive: t.IsActive,
            Notes: t.Notes
        )).ToList();

        logger.LogInformation(CustomerTablesLoadedEvent, "Customer tables loaded for player {PlayerId}: {Count} tables", userId, items.Count);

        return Ok(new CustomerTablesResponse(items));
    }

    /// <summary>
    /// ดึงข้อมูลโต๊ะลูกค้าตาม ID
    /// GET /api/v1/customer-tables/{tableId}
    /// </summary>
    [HttpGet("{tableId:guid}")]
    [ProducesResponseType(typeof(CustomerTableItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerTableItem>> GetCustomerTable(Guid tableId, CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(subject) || !Guid.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        var user = await database.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null) return Unauthorized();
        if (user.IsSuspended) return ForbiddenProblem();

        var table = await database.CustomerTables
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.PK == tableId && t.Status == "ACTIVE", cancellationToken);

        if (table is null)
        {
            return NotFound(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = "Customer table not found."
            });
        }

        var item = new CustomerTableItem(
            Id: table.PK,
            TableNumber: table.TableNumber,
            Capacity: table.Capacity,
            InitialStatus: table.InitialStatus,
            AssignedCustomerType: table.AssignedCustomerType,
            AssignedCustomerId: table.AssignedCustomerId,
            PositionX: table.PositionX,
            PositionY: table.PositionY,
            Zone: table.Zone,
            IsActive: table.IsActive,
            Notes: table.Notes
        );

        logger.LogInformation(CustomerTableLoadedEvent, "Customer table {TableId} loaded for player {PlayerId}", tableId, userId);

        return Ok(item);
    }

    /// <summary>
    /// ดึงสรุปสถานะโต๊ะ (table count, status summary)
    /// GET /api/v1/customer-tables/summary
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(CustomerTableSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CustomerTableSummaryResponse>> GetCustomerTableSummary(CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(subject) || !Guid.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        var user = await database.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null) return Unauthorized();
        if (user.IsSuspended) return ForbiddenProblem();

        var tables = await database.CustomerTables
            .AsNoTracking()
            .Where(t => t.Status == "ACTIVE" && t.IsActive)
            .ToListAsync(cancellationToken);

        var summary = new CustomerTableSummaryResponse(
            TotalTables: tables.Count,
            EmptyTables: tables.Count(t => t.InitialStatus == "EMPTY"),
            OccupiedTables: tables.Count(t => t.InitialStatus == "OCCUPIED"),
            ReservedTables: tables.Count(t => t.InitialStatus == "RESERVED"),
            DirtyTables: tables.Count(t => t.InitialStatus == "DIRTY"),
            TotalCapacity: tables.Sum(t => t.Capacity),
            ByZone: tables
                .GroupBy(t => t.Zone ?? "DEFAULT")
                .Select(g => new ZoneTableCount(g.Key, g.Count()))
                .ToList(),
            ByCustomerType: tables
                .Where(t => !string.IsNullOrEmpty(t.AssignedCustomerType))
                .GroupBy(t => t.AssignedCustomerType!)
                .Select(g => new CustomerTypeTableCount(g.Key, g.Count()))
                .ToList()
        );

        return Ok(summary);
    }

    private ObjectResult ForbiddenProblem() => StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
    {
        Type = "https://tools.ietf.org/html/rfc7807",
        Title = "Forbidden",
        Status = StatusCodes.Status403Forbidden,
        Detail = "Account is suspended."
    });

    // DTO Records
    public sealed record CustomerTablesResponse(List<CustomerTableItem> Tables);
    
    public sealed record CustomerTableItem(
        Guid Id,
        int TableNumber,
        int Capacity,
        string InitialStatus,
        string? AssignedCustomerType,
        int? AssignedCustomerId,
        int PositionX,
        int PositionY,
        string? Zone,
        bool IsActive,
        string? Notes);

    public sealed record CustomerTableSummaryResponse(
        int TotalTables,
        int EmptyTables,
        int OccupiedTables,
        int ReservedTables,
        int DirtyTables,
        int TotalCapacity,
        List<ZoneTableCount> ByZone,
        List<CustomerTypeTableCount> ByCustomerType);

    public sealed record ZoneTableCount(string Zone, int Count);
    public sealed record CustomerTypeTableCount(string CustomerType, int Count);
}