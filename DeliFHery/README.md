# DeliFHery

The infrastructure assets that provision PostgreSQL, Keycloak, and the API container now live under [`DeliFHery/infrastructure`](infrastructure). You can launch the exact same stack from the repository root via [`infrastructure/docker-compose.yml`](../infrastructure/docker-compose.yml) if you prefer; both compose files are kept in sync. Use whichever compose file matches the directory you are working from to run the full stack locally:

```bash
cd DeliFHery
docker compose -f infrastructure/docker-compose.yml up --build
```

See [`../infrastructure/README.md`](../infrastructure/README.md) for a full breakdown of each Docker service, the published ports, and troubleshooting tips (including what it means when `docker ps` shows multiple API containers). At a glance, the compose stack exposes:

- PostgreSQL on `localhost:5432`
- Keycloak on `http://localhost:8080`
- The API container listens on port `8080`, which Docker forwards to `http://localhost:5000` on your machine. Swagger UI is therefore available at `http://localhost:5000/swagger` once the API container reports `Now listening on: http://[::]:8080` in the logs. The compose stack does **not** configure HTTPS endpoints, so make sure you access the API via `http://` rather than `https://`.

Environment variables inside the compose file already match the configuration expected by `DeliFHery.Api`, so no additional setup is required beyond having Docker Desktop running.

### Verifying the exposed URLs

If you're unsure which URLs/ports ended up being published locally, Docker Compose can print them for you:

```bash
# Lists every service, the container port, and the bound host port
docker compose -f infrastructure/docker-compose.yml ps

# Shows the host port that forwards to the container port 8080 (the ASP.NET API)
docker compose -f infrastructure/docker-compose.yml port api 8080
```

Use the host port reported by these commands when calling the API or opening Swagger in your browser. When the logs mention `Binding to values defined by URLS instead 'http://+:8080'`, it only refers to the internal container port; the host-facing port remains whatever `docker compose ps` reports (5000 by default). If you see an extra container named `DeliFHery.Api` when running `docker ps`, it simply means another process started a separate container with its own port (for example, `dotnet run` may publish `localhost:32806`). Either stop that container or browse to the specific port it prints; it does not affect the Compose-managed port.

If you need to change the published host port (for example, to avoid clashing with something already running on `localhost:5000`), edit the `api` service's `ports` section in [`infrastructure/docker-compose.yml`](infrastructure/docker-compose.yml). The syntax is `HOST_PORT:CONTAINER_PORT`, so changing it to `6000:8080` would expose the API (and Swagger UI) at `http://localhost:6000` instead.
