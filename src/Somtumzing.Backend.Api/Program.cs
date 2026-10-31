using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Middleware;
using Somtumzing.Backend.Api.Models;
using Somtumzing.Backend.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole(options =>
{
    options.IncludeScopes = true;
    options.UseUtcTimestamp = true;
});

var connectionString = builder.Configuration.GetConnectionString("GameDatabase")
    ?? throw new InvalidOperationException("ConnectionStrings:GameDatabase is required.");
var signingKeyValue = builder.Configuration["Jwt:SigningKeyBase64"]
    ?? throw new InvalidOperationException("Jwt:SigningKeyBase64 is required.");

byte[] signingKey;
try
{
    signingKey = Convert.FromBase64String(signingKeyValue);
}
catch (FormatException exception)
{
    throw new InvalidOperationException("Jwt:SigningKeyBase64 must be valid Base64.", exception);
}

if (signingKey.Length < 32)
{
    throw new InvalidOperationException("Jwt:SigningKeyBase64 must decode to at least 32 bytes.");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "somtumzing-backend";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "somtumzing-game";
var auditRetentionDays = builder.Configuration.GetValue<int?>("AuditLogs:RetentionDays") ?? 180;
if (auditRetentionDays < 1)
{
    throw new InvalidOperationException("AuditLogs:RetentionDays must be greater than zero.");
}

builder.Services.AddDbContext<GameDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IAuditLogWriter, AuditLogWriter>();
builder.Services.AddHostedService<AuditLogRetentionService>();
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequiredLength = 12;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddEntityFrameworkStores<GameDbContext>()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "Somtumzing.BackOffice";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.LoginPath = "/Admin/Login";
    options.AccessDeniedPath = "/Admin/Forbidden";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var subject = context.Principal?.FindFirst("sub")?.Value;
                var securityStamp = context.Principal?.FindFirst("sst")?.Value;
                if (!Guid.TryParse(subject, out var playerId)
                    || string.IsNullOrWhiteSpace(securityStamp))
                {
                    context.Fail("The player token is invalid.");
                    return;
                }

                var database = context.HttpContext.RequestServices.GetRequiredService<GameDbContext>();
                var player = await database.Users
                    .AsNoTracking()
                    .Where(user => user.Id == playerId)
                    .Select(user => new { user.IsSuspended, user.SecurityStamp })
                    .SingleOrDefaultAsync(context.HttpContext.RequestAborted);

                if (player is null || player.IsSuspended || player.SecurityStamp != securityStamp)
                {
                    context.Fail("The player account is suspended or the token has been revoked.");
                }
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKey),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BackOffice", policy =>
    {
        policy.AddAuthenticationSchemes(IdentityConstants.ApplicationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin");
    });
});
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "BackOffice");
    options.Conventions.AllowAnonymousToPage("/Admin/Login");
});
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddTransient<RequestLoggingMiddleware>();
builder.Services.AddSingleton(new AccessTokenService(signingKey, jwtIssuer, jwtAudience));
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("postgres", tags: ["ready"]);
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("guest-bootstrap", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    await DatabaseInitializer.InitializeAsync(scope.ServiceProvider, app.Configuration);
}

app.UseExceptionHandler();
app.Use(async (context, next) =>
{
    using var scope = app.Logger.BeginScope(new Dictionary<string, object>
    {
        ["RequestId"] = context.TraceIdentifier
    });
    await next(context);
});
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health/live", new()
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready", new()
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapControllers();
app.MapRazorPages();

app.Run();

public partial class Program;
