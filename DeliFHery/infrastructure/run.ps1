param(
    [switch]$ResetKeycloakVolume,
    [switch]$NoLogs
)

# Resolve repo root and compose path (use absolute paths so the script works from anywhere)
$ScriptDir = $PSScriptRoot
$RepoRoot = Split-Path -Parent $ScriptDir
$ComposeFile = Join-Path $ScriptDir "docker-compose.yml"

if (-not (Test-Path $ComposeFile)) {
    throw "Compose file not found at '$ComposeFile'. Ensure you cloned the repo and are running the script from the DeliFHery project."
}

Push-Location $RepoRoot
try {
    if ($ResetKeycloakVolume) {
        Write-Host "Removing Keycloak volume to force realm import..." -ForegroundColor Yellow
        docker volume rm delifhery-platform_keycloak-data -f | Out-Null
    }

    Write-Host "Building and starting containers (detached)..." -ForegroundColor Cyan
    docker compose -f "$ComposeFile" up --build -d

    Write-Host "\nContainer status:" -ForegroundColor Cyan
    docker compose -f "$ComposeFile" ps

    Write-Host "\nWaiting for Keycloak to become ready (up to 3 minutes)..." -ForegroundColor Cyan
    $sw = [Diagnostics.Stopwatch]::StartNew()
    while ($sw.Elapsed.TotalSeconds -lt 180) {
        try {
            Invoke-RestMethod -UseBasicParsing -Method Get -Uri 'http://localhost:8080/realms/delifhery/.well-known/openid-configuration' -TimeoutSec 5 | Out-Null
            Write-Host 'Keycloak is ready.' -ForegroundColor Green
            break
        } catch {
            Start-Sleep -Seconds 3
        }
    }

    Write-Host "Requesting token for 'dispatcher' using curl.exe..." -ForegroundColor Cyan
    curl.exe -X POST "http://localhost:8080/realms/delifhery/protocol/openid-connect/token" `
      -H "Content-Type: application/x-www-form-urlencoded" `
      -d "grant_type=password" `
      -d "client_id=delifhery-web" `
      -d "username=dispatcher" `
      -d "password=ChangeMe123!"

    if (-not $NoLogs) {
        Write-Host "\nFollowing API and Keycloak logs (Ctrl+C to exit)..." -ForegroundColor Cyan
        docker compose -f "$ComposeFile" logs -f api keycloak
    }
}
finally {
    Pop-Location
}
