[CmdletBinding()]
param()

$ScriptDir = $PSScriptRoot
$RepoRoot = Split-Path -Parent $ScriptDir
$ComposeFile = Join-Path $ScriptDir "docker-compose.yml"

if (-not (Test-Path $ComposeFile)) {
    throw "Compose file not found at '$ComposeFile'. Ensure you cloned the repo and are running the script from the DeliFHery project."
}

Push-Location $RepoRoot
try {
    Write-Host "Stopping containers and removing networks/volumes..." -ForegroundColor Cyan
    docker compose -f "$ComposeFile" down --volumes --remove-orphans

    $volumePrefix = 'delifhery-platform'
    $volumes = docker volume ls -q --filter "name=$volumePrefix"
    foreach ($volume in $volumes) {
        docker volume rm $volume -f | Out-Null
    }

    $networks = docker network ls -q --filter "name=$volumePrefix"
    foreach ($network in $networks) {
        docker network rm $network -f | Out-Null
    }

    Write-Host "Docker resources removed." -ForegroundColor Green
}
finally {
    Pop-Location
}
