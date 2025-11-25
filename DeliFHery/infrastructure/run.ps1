param(
    [switch]$ResetKeycloakVolume
)

# Resolve repo root and compose path
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$RepoRoot = Split-Path -Parent $ScriptDir
$ComposeFile = Join-Path $ScriptDir "docker-compose.yml"

Set-Location $RepoRoot

if ($ResetKeycloakVolume) {
    Write-Host "Removing Keycloak volume to force realm import..." -ForegroundColor Yellow
    docker volume rm delifhery-platform_keycloak-data -f | Out-Null
}

# Start docker compose (build + up)
$composeCmd = "docker compose -f `"$ComposeFile`" up --build"
Start-Process powershell -ArgumentList "-NoExit", "-Command", $composeCmd

# Show port mappings
$portsCmd = "docker compose -f `"$ComposeFile`" ps"
Start-Process powershell -ArgumentList "-NoExit", "-Command", $portsCmd

# Tail API + Keycloak logs
$logsCmd = "docker compose -f `"$ComposeFile`" logs -f api keycloak"
Start-Process powershell -ArgumentList "-NoExit", "-Command", $logsCmd

# Token helper window: wait until Keycloak is reachable, then fetch a token.
$tokenCmd = @"
Write-Host 'Waiting for Keycloak to become ready (up to 3 minutes)...'
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
"@
Start-Process powershell -ArgumentList "-NoExit", "-Command", $tokenCmd
