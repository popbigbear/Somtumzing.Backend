$ErrorActionPreference = "Stop"

$repositoryRoot = $PSScriptRoot
$environmentFile = Join-Path $repositoryRoot ".env"
$dotnet = $null
$project = Join-Path $repositoryRoot "src\Somtumzing.Backend.Api\Somtumzing.Backend.Api.csproj"
$efProject = "src\Somtumzing.Backend.Api\Somtumzing.Backend.Api.csproj"

if (-not (Test-Path -LiteralPath $environmentFile)) {
    throw "Local secrets file was not found: $environmentFile"
}
foreach ($line in Get-Content -LiteralPath $environmentFile) {
    if ([string]::IsNullOrWhiteSpace($line) -or $line.StartsWith("#")) {
        continue
    }

    $separator = $line.IndexOf("=")
    if ($separator -lt 1) {
        throw "Invalid setting in local secrets file."
    }

    $name = $line.Substring(0, $separator)
    $value = $line.Substring($separator + 1)
    if ($name -notmatch '^[A-Za-z_][A-Za-z0-9_]*$') {
        throw "Invalid environment variable name in local secrets file."
    }

    Set-Item -Path "Env:$name" -Value $value
}

$dotnetCommand = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not [string]::IsNullOrWhiteSpace($env:DOTNET_EXE)) {
    $dotnet = $env:DOTNET_EXE
}
elseif ($null -ne $dotnetCommand) {
    $dotnet = $dotnetCommand.Source
}
else {
    throw "The .NET 10 SDK was not found. Install it or set DOTNET_EXE in .env."
}

if (-not (Test-Path -LiteralPath $dotnet -PathType Leaf)) {
    throw "The configured .NET executable was not found."
}

$env:DOTNET_CLI_HOME = if ([string]::IsNullOrWhiteSpace($env:DOTNET_CLI_HOME)) {
    Join-Path $env:LOCALAPPDATA "Somtumzing\.dotnet-cli-home"
}
else {
    $env:DOTNET_CLI_HOME
}
$env:NUGET_PACKAGES = if ([string]::IsNullOrWhiteSpace($env:NUGET_PACKAGES)) {
    Join-Path $env:USERPROFILE ".nuget\packages"
}
else {
    $env:NUGET_PACKAGES
}
$env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"
$env:ASPNETCORE_ENVIRONMENT = "Development"

Push-Location $repositoryRoot
try {
    & $dotnet tool restore
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }

    & $dotnet tool run dotnet-ef database update --project $efProject
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }

    & $dotnet run --project $project --urls "http://127.0.0.1:5184"
    exit $LASTEXITCODE
}
finally {
    Pop-Location
}
