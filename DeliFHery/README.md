# DeliFHery

The infrastructure assets that provision PostgreSQL, Keycloak, and the API container now live under [`DeliFHery/infrastructure`](infrastructure). Use the compose file there to run the full stack locally:

```bash
cd DeliFHery
docker compose -f infrastructure/docker-compose.yml up --build
```

The compose stack exposes:

- PostgreSQL on `localhost:5432`
- Keycloak on `http://localhost:8080`
- The API on `http://localhost:5000`

Environment variables inside the compose file already match the configuration expected by `DeliFHery.Api`, so no additional setup is required beyond having Docker Desktop running.
