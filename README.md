# Somtumzing Backend

ASP.NET Core 10 backend for the Somtumzing mobile game. This repository is independent of `gold-ai-agent`.

## Prerequisites

- .NET 10 SDK
- Docker Desktop with Docker Compose

## Start PostgreSQL

Docker Compose is the recommended local setup. Copy `.env.example` to `.env`, replace all `REPLACE_*` values with unique secrets, then start PostgreSQL:

```powershell
Copy-Item .env.example .env
docker compose up -d postgres
```

The Docker database is bound to localhost only. Generate a JWT key with PowerShell and put the result in `Jwt__SigningKeyBase64` in `.env`:

```powershell
$key = New-Object byte[] 48
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
try { $rng.GetBytes($key) } finally { $rng.Dispose() }
[Convert]::ToBase64String($key)
```

`POSTGRES_PASSWORD` and the password in `ConnectionStrings__GameDatabase` must match.

For a native Windows PostgreSQL installation, install PostgreSQL 17 from an elevated PowerShell prompt:

```powershell
winget install --id PostgreSQL.PostgreSQL.17 --exact --silent --accept-package-agreements --accept-source-agreements
```

The installer only installs PostgreSQL; it does not apply this project's loopback-only network policy or provision the application database role. Configure those securely with your PostgreSQL administrator before starting the API, then set the matching local connection string and secrets in `.env`.

## Configure and run the API

After PostgreSQL has been secured and `.env` has been configured, start the API with:

```powershell
.\run-local.ps1
```

The script uses `dotnet` from `PATH`, or an optional `DOTNET_EXE` setting in `.env`, applies EF migrations, and starts the API at `http://127.0.0.1:5184`. Open `/Admin/Login` for the back-office, or use `GET /api/v1/system/status`.

The first run applies the EF migration and creates the configured back-office administrator. Keep `.env` private; do not commit it.

## Initial endpoints

- `POST /api/v1/auth/guest` creates a guest player and returns a short-lived access token plus a rotating refresh token.
- `POST /api/v1/auth/refresh` rotates a refresh token; only its SHA-256 hash is stored.
- `POST /api/v1/auth/logout` revokes the current refresh token for the authenticated player.
- `GET /api/v1/auth/me` returns the authenticated player's profile.
- `GET /health/live` checks that the process is running.
- `GET /health/ready` checks PostgreSQL connectivity.

Guest creation is rate-limited by client IP. Guest access tokens expire after 15 minutes and refresh tokens after 30 days. Linking guest accounts to Apple or Google is a follow-up integration; the API does not yet accept or validate those providers' credentials.
Logging out revokes future refreshes for that device; an already-issued access token remains valid until its 15-minute expiry.

## Logging

Application logs use the built-in .NET six-level severity scale: `Trace`, `Debug`, `Information`, `Warning`, `Error`, and `Critical`. The separate `EventType` property identifies the business/operational event (for example `PlayerLogin`, `AdminLogin`, `DatabaseFailure`, or `UnhandledException`).

Logs are written as structured JSON to standard output. The API records request method, path, status, and elapsed time without query strings or request bodies, and includes request/trace IDs in scopes. The local `run-local.ps1` console shows the logs; production hosting should collect and retain stdout logs in its log platform. Do not log passwords, access or refresh tokens, request bodies, or connection strings. Unexpected exceptions return a generic problem response with a trace ID while the full exception is logged server-side.

Severity filtering is configuration-driven:

- Development: `Somtumzing.Backend` defaults to `Debug`; framework categories default to `Warning`.
- Other environments: `Somtumzing.Backend` defaults to `Information`; framework categories default to `Warning`.
- Override a category with configuration. For example, temporarily run `Set-Item Env:Logging__LogLevel__Somtumzing.Backend -Value Trace` before starting the API to enable detailed diagnostics. Trace logging can expose sensitive implementation data and should not remain enabled in production.

The API currently emits operational/authentication events only. Durable game transaction ledgers and back-office audit records belong in domain tables written atomically with the corresponding business changes; they are not replaced by operational logs and will be introduced alongside those game/admin operations.

## Migrations

When changing the EF Core model, create and apply migrations:

```powershell
$dotnet = "D:\bbssweb\dotnet-sdk-10.0.401\dotnet.exe"
& $dotnet tool run dotnet-ef migrations add <MigrationName> --project src/Somtumzing.Backend.Api
& $dotnet tool run dotnet-ef database update --project src/Somtumzing.Backend.Api
```

Migrations are applied explicitly rather than automatically at application startup.
