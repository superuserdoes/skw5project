# DeliFHery API

## Configuration profiles

The API reads the standard `appsettings.json` (and environment-specific variants) during startup.
When the application detects that it is running inside a container (`DOTNET_RUNNING_IN_CONTAINER=true`) it also loads `appsettings.Container.json`.

`appsettings.Container.json` points the API to the Postgres and Keycloak services that are shipped alongside the API image (for example via `docker compose`).
This allows running the API container by itself without setting extra environment variables—the bundled Postgres service is reachable as `database` and Keycloak is reachable as `keycloak`.

## Running the full stack with Docker Compose

The repository ships with an infrastructure compose file that builds and runs the Postgres, Keycloak, and API containers together. Follow these steps from the repository root:

1. Build and start everything: `docker compose -f infrastructure/docker-compose.yml up --build`
   * **database** – Postgres 16 with the seeded `init.sql`, exposed on `localhost:5432`.
   * **keycloak** – Keycloak 24 running the imported `delifhery` realm, exposed on `http://localhost:8080`.
   * **api** – The DeliFHery API container (listening on port 8080 inside the container) exposed as `http://localhost:5000`.
2. Wait for Compose to report healthy containers. The database service has an explicit health check, and the API depends on both the database and Keycloak becoming ready. You can watch the status via `docker compose -f infrastructure/docker-compose.yml ps` and look for the `healthy` flag.
3. Once all services are healthy:
   * Browse to `http://localhost:5000/swagger` (or other API endpoints) to exercise the API via your browser.
   * Use Postman or another HTTP client to send requests to `http://localhost:5000`, optionally using Keycloak at `http://localhost:8080` to obtain tokens for protected endpoints.

Shutting the stack down is as simple as pressing `Ctrl+C` in the compose terminal or running `docker compose -f infrastructure/docker-compose.yml down` from another shell.
