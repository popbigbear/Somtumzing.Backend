using Microsoft.AspNetCore.Identity;

namespace Somtumzing.Backend.Api.Models;

public sealed class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = "ACTIVE";
}

public sealed class ApplicationUserRole : IdentityUserRole<Guid>
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = "ACTIVE";
}
