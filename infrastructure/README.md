# Infrastructure stack

This repository contains two identical Docker Compose files so you can run the stack either from the repository root (`infrastructure/docker-compose.yml`) or from within the `DeliFHery` sub-project (`DeliFHery/infrastructure/docker-compose.yml`). Both files define the exact same services and ports – choose whichever is most convenient for where you are working from.

## Services and ports

| Service  | Compose name | Host port → container port | Purpose |
|----------|--------------|----------------------------|---------|
| PostgreSQL | `database` | `5432 → 5432` | Persists the application data. Credentials are defined via `POSTGRES_USER/PASSWORD/DB` env vars. |
| Keycloak | `keycloak` | `8080 → 8080` | Provides authentication/authorization via the bundled realm import. |
| ASP.NET API | `api` | `5000 → 8080` | Hosts `DeliFHery.Api`. The container binds to `http://+:8080` internally, and Docker publishes it to `http://localhost:5000` by default. |

There are no conflicting port mappings across the compose files – the mappings above are identical in both compose definitions, so you will always get the same host endpoints regardless of which compose file you launch.

## Running the stack

From the repository root:

```bash
# build the API image and start postgres, keycloak, and the API
cd /workspace/skw5project
docker compose -f infrastructure/docker-compose.yml up --build
```

Alternatively, if you prefer to work from inside the `DeliFHery` directory, run:

```bash
cd /workspace/skw5project/DeliFHery
docker compose -f infrastructure/docker-compose.yml up --build
```

Either command sequence will produce the same containers and host port bindings.

## Verifying that the services are reachable

1. Wait for the `delifhery-api` logs to print `Now listening on: http://[::]:8080`. That means Swagger will be available at `http://localhost:5000/swagger`.
2. Use `docker compose -f infrastructure/docker-compose.yml ps` to view the host→container port mappings.
3. Use `docker compose -f infrastructure/docker-compose.yml logs -f api` to follow the API logs if you need to troubleshoot.

Because HTTPS is not configured in the compose files, make sure you access the API over plain HTTP. If port `5000` is already occupied on your machine, edit the `ports` entry of the `api` service (e.g., change it to `6000:8080`) in whichever compose file you are using and re-run `docker compose up`.

### When `docker ps` shows multiple API containers

If you see both `delifhery-api` **and** something like `DeliFHery.Api` (with an image such as `delifheryapi:dev`) in `docker ps`, that means you have another container running outside of Compose (for example, one started by `dotnet run` or Visual Studio). Each container binds its own host port, which explains why you might notice an extra mapping such as `0.0.0.0:32806->8080/tcp` alongside the Compose-managed `0.0.0.0:5000->8080/tcp`.

To know which URL to hit, always rely on the Compose metadata:

```bash
docker compose -f infrastructure/docker-compose.yml ps
docker compose -f infrastructure/docker-compose.yml port api 8080
```

These commands report the host port (5000 unless you edited the compose file) that forwards traffic to the API container inside the stack. You can either stop the extra `DeliFHery.Api` container (`docker stop DeliFHery.Api`) or leave it running and explicitly browse to the port it exposes (e.g., `http://localhost:32806/swagger`).

### Manually testing the endpoints

Once the stack is up and the port mapping is confirmed, use either a browser or any HTTP client against the host port:

```bash
# Open the interactive Swagger UI in a browser
http://localhost:5000/swagger

# Or call the API directly via curl (replace the path with an actual endpoint)
curl http://localhost:5000/api/health
```

Swap `5000` for whatever `docker compose port api 8080` prints if you customized the mapping.
