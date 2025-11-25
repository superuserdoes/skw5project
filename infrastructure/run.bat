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
start "delifhery-token" cmd /k "curl -X POST http://localhost:8080/realms/delifhery/protocol/openid-connect/token -H "Content-Type: application/x-www-form-urlencoded" -d "grant_type=password" -d "client_id=delifhery-web" -d "username=dispatcher" -d "password=ChangeMe123!""

popd
endlocal
