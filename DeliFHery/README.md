# DeliFHery

The infrastructure assets that provision PostgreSQL, Keycloak, and the API container now live under [`DeliFHery/infrastructure`](infrastructure). Use the compose file there to run the full stack locally:

```bash
cd DeliFHery
docker compose -f infrastructure/docker-compose.yml up --build
```

The compose stack exposes:

- PostgreSQL on `localhost:5432`
- Keycloak on `http://localhost:8080`
- The API container listens on port `8080`, which Docker forwards to `http://localhost:5000` on your machine. Swagger UI is therefore available at `http://localhost:5000/swagger` once the API container reports `Now listening on: http://[::]:8080` in the logs.

Environment variables inside the compose file already match the configuration expected by `DeliFHery.Api`, so no additional setup is required beyond having Docker Desktop running.

### Verifying the exposed URLs

If you're unsure which URLs/ports ended up being published locally, Docker Compose can print them for you:

```bash
# Lists every service, the container port, and the bound host port
docker compose -f infrastructure/docker-compose.yml ps

# Shows the host port that forwards to the container port 8080 (the ASP.NET API)
docker compose -f infrastructure/docker-compose.yml port api 8080
```

Use the host port reported by these commands when calling the API or opening Swagger in your browser. When the logs mention `Binding to values defined by URLS instead 'http://+:8080'`, it only refers to the internal container port; the host-facing port remains whatever `docker compose ps` reports (5000 by default).
