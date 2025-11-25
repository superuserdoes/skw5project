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

    Write-Host "Requesting token for 'dispatcher' and copying to clipboard..." -ForegroundColor Cyan
    $tokenResponse = Invoke-RestMethod -UseBasicParsing -Method Post `
        -Uri 'http://localhost:8080/realms/delifhery/protocol/openid-connect/token' `
        -ContentType 'application/x-www-form-urlencoded' `
        -Body @{
            grant_type = 'password'
            client_id = 'delifhery-web'
            username = 'dispatcher'
            password = 'ChangeMe123!'
        }

    if ($tokenResponse.access_token) {
        Set-Clipboard -Value $tokenResponse.access_token
        Write-Host "Access token copied to clipboard." -ForegroundColor Green
        Write-Host "Token snippet (first 40 chars): $($tokenResponse.access_token.Substring(0, [Math]::Min(40, $tokenResponse.access_token.Length)))..." -ForegroundColor DarkGray
    } else {
        Write-Warning "Token response did not include an access_token."
    }

    if (-not $NoLogs) {
        Write-Host "\nFollowing API and Keycloak logs (Ctrl+C to exit)..." -ForegroundColor Cyan
        docker compose -f "$ComposeFile" logs -f api keycloak
    }
}
finally {
    Pop-Location
}
