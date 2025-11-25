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

# Token helper (default scopes included automatically, no scope param needed)
$tokenCmd = @"
curl -X POST http://localhost:8080/realms/delifhery/protocol/openid-connect/token `
  -H 'Content-Type: application/x-www-form-urlencoded' `
  -d 'grant_type=password' `
  -d 'client_id=delifhery-web' `
  -d 'username=dispatcher' `
  -d 'password=ChangeMe123!'
"@
Start-Process powershell -ArgumentList "-NoExit", "-Command", $tokenCmd
