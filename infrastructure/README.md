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

## Getting a Keycloak token for the API

The API enforces the `delifhery-api` scope. A matching client scope is baked into the realm import, so you can request it from the bundled Keycloak instance without any manual setup.

1. Make sure the stack is running (see commands above).
2. If you ever see `invalid_scope` when requesting a token, it usually means your `keycloak-data` volume still holds an older realm that predates the `delifhery-api` scope. Remove the volume so Keycloak re-imports the updated realm on next start:

   - **PowerShell / WSL / Git Bash:**
     ```bash
     docker compose -f infrastructure/docker-compose.yml down -v
     ```

   - **cmd.exe:**
     ```bat
     docker compose -f infrastructure\docker-compose.yml down -v
     ```

   Then start the stack again: `docker compose -f infrastructure/docker-compose.yml up --build`.

3. Request an access token using the seeded user and the `delifhery-web` client. Examples for each shell:

   - **PowerShell** (recommended on modern Windows):
     ```powershell
     curl -Method POST http://localhost:8080/realms/delifhery/protocol/openid-connect/token `
       -Headers @{ "Content-Type" = "application/x-www-form-urlencoded" } `
       -Body "grant_type=password&client_id=delifhery-web&username=dispatcher&password=ChangeMe123!&scope=delifhery-api"
     ```

   - **cmd.exe** (Command Prompt):
     ```bat
     curl -X POST http://localhost:8080/realms/delifhery/protocol/openid-connect/token ^
       -H "Content-Type: application/x-www-form-urlencoded" ^
       -d "grant_type=password" ^
       -d "client_id=delifhery-web" ^
       -d "username=dispatcher" ^
       -d "password=ChangeMe123!" ^
       -d "scope=delifhery-api"
     ```

   - **bash / WSL / Git Bash**:
     ```bash
     curl -X POST http://localhost:8080/realms/delifhery/protocol/openid-connect/token \
       -H "Content-Type: application/x-www-form-urlencoded" \
       -d "grant_type=password" \
       -d "client_id=delifhery-web" \
       -d "username=dispatcher" \
       -d "password=ChangeMe123!" \
       -d "scope=delifhery-api"
     ```

   Keycloak rejects unknown scopes, so stick to `delifhery-api` (and avoid shell line-break issues by using the variant that matches your terminal). The response JSON contains `access_token`. Copy that value.

   > Tip: The realm export now ships `delifhery-api` as a **default** client scope, so if you omit the `scope` line entirely the token will still include it after you’ve refreshed the Keycloak volume.

3. Call the API with the token (replace `5000` if your port differs):

   ```bash
   curl http://localhost:5000/api/deliveries \
     -H "Authorization: Bearer <access_token>"
   ```

If you omit the `delifhery-api` scope, Keycloak will reject the request with `invalid_scope` and the API will respond with `Unauthorized`.

### Refreshing the imported realm (to pick up changes)

The Keycloak data is stored in the `keycloak-data` Docker volume. If you started the stack before the `delifhery-api` scope was added, Keycloak will skip re-importing the realm and you will still have the old configuration. To force a fresh import:

```bash
docker compose -f infrastructure/docker-compose.yml down -v   # removes containers AND volumes
docker compose -f infrastructure/docker-compose.yml up --build
```

This wipes the persisted realm, re-imports `realm-export.json`, and ensures the `delifhery-api` scope and user are available. You can also remove just the Keycloak volume without touching Postgres data via `docker volume rm delifhery-platform_keycloak-data`.
