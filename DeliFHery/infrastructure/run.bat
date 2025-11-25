@echo off
setlocal

set COMPOSE_FILE=%~dp0docker-compose.yml
pushd %~dp0\..

if /i "%1"=="--reset-keycloak" (
  echo Removing Keycloak volume to force realm import...
  docker volume rm delifhery-platform_keycloak-data -f >nul
)

start "delifhery-compose" cmd /k "docker compose -f "%COMPOSE_FILE%" up --build"
start "delifhery-ports" cmd /k "docker compose -f "%COMPOSE_FILE%" ps"
start "delifhery-logs" cmd /k "docker compose -f "%COMPOSE_FILE%" logs -f api keycloak"
start "delifhery-token" cmd /k "powershell -NoProfile -Command ^
  \"Write-Host 'Waiting for Keycloak to become ready (up to 3 minutes)...'; ^
  $sw=[Diagnostics.Stopwatch]::StartNew(); ^
  while($sw.Elapsed.TotalSeconds -lt 180){ ^
    try{Invoke-RestMethod -UseBasicParsing -Method Get -Uri 'http://localhost:8080/realms/delifhery/.well-known/openid-configuration' -TimeoutSec 5 ^| Out-Null; Write-Host 'Keycloak is ready.' -ForegroundColor Green; break;} ^
    catch{Start-Sleep -Seconds 3;} ^
  } ^
  Write-Host 'Requesting token for dispatcher using curl.exe...' -ForegroundColor Cyan; ^
  curl.exe -X POST http://localhost:8080/realms/delifhery/protocol/openid-connect/token ^
    -H \"Content-Type: application/x-www-form-urlencoded\" ^
    -d \"grant_type=password\" ^
    -d \"client_id=delifhery-web\" ^
    -d \"username=dispatcher\" ^
    -d \"password=ChangeMe123!\"\""

popd
endlocal
