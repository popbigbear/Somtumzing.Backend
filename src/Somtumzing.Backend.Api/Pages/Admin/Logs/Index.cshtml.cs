using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Api.Pages.Admin.Logs;

public sealed class IndexModel(GameDbContext database) : PageModel
{
    private const int PageSize = 50;

    [BindProperty(SupportsGet = true)]
    public string? EventType { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Severity { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? From { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? To { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? IpAddress { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Username { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public IReadOnlyList<AuditLogEntry> Entries { get; private set; } = [];
    public IReadOnlyList<string> EventTypes { get; private set; } = [];
    public int TotalCount { get; private set; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    public int CurrentPage => Math.Clamp(PageNumber, 1, TotalPages);

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        EventTypes = await database.AuditLogEntries
            .AsNoTracking()
            .Select(entry => entry.EventType)
            .Distinct()
            .OrderBy(eventType => eventType)
            .ToListAsync(cancellationToken);

        if (From > To)
        {
            ModelState.AddModelError(string.Empty, "'From' must be on or before 'To'.");
            return;
        }

        if (To == DateOnly.MaxValue)
        {
            ModelState.AddModelError(nameof(To), "'To' must be earlier than the maximum date.");
            return;
        }

        var query = database.AuditLogEntries.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(EventType))
        {
            query = query.Where(entry => entry.EventType == EventType);
        }

        if (!string.IsNullOrWhiteSpace(Severity))
        {
            query = query.Where(entry => entry.Severity == Severity);
        }

        if (From is { } from)
        {
            var startUtc = from.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            query = query.Where(entry => entry.OccurredAtUtc >= startUtc);
        }

        if (To is { } to)
        {
            var endUtcExclusive = to.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            query = query.Where(entry => entry.OccurredAtUtc < endUtcExclusive);
        }

        if (!string.IsNullOrWhiteSpace(IpAddress))
        {
            query = query.Where(entry => entry.IpAddress == IpAddress);
        }

        if (!string.IsNullOrWhiteSpace(Username))
        {
            query = query.Where(entry => entry.ActorUsername != null
                && entry.ActorUsername.Contains(Username));
        }

        TotalCount = await query.CountAsync(cancellationToken);
        PageNumber = Math.Clamp(PageNumber, 1, TotalPages);
        Entries = await query
            .OrderByDescending(entry => entry.OccurredAtUtc)
            .ThenByDescending(entry => entry.Id)
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);
    }
}
