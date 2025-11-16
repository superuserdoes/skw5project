# DeliFHery API

## Configuration profiles

The API reads the standard `appsettings.json` (and environment-specific variants) during startup.
When the application detects that it is running inside a container (`DOTNET_RUNNING_IN_CONTAINER=true`) it also loads `appsettings.Container.json`.

`appsettings.Container.json` points the API to the Postgres and Keycloak services that are shipped alongside the API image (for example via `docker compose`).
This allows running the API container by itself without setting extra environment variables—the bundled Postgres service is reachable as `database` and Keycloak is reachable as `keycloak`.
