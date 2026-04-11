# Quick Start Guide

## Option A — Aspire (recommended for development)

```bash
cd AppHost
dotnet run --launch-profile https
```

Aspire Dashboard: `https://localhost:15889`

Aspire starts all services automatically, injects service URLs, and shows live logs, traces, and metrics.

> Prerequisites: .NET 10 SDK, Docker Desktop (for Redis container)

---

## Option B — Docker Compose (local full stack)

```bash
# From repo root — builds all images and starts everything
docker compose up --build

# Rebuild a single service after code changes
docker compose up --build coursemanagement

# Stop
docker compose down

# Stop and wipe database volumes
docker compose down -v
```

### Service URLs

| Service | URL |
|---|---|
| Web App | http://localhost:5173 |
| Course Management API | http://localhost:8080/swagger |
| User Management API | http://localhost:8081/swagger |
| Discount gRPC | http://localhost:8083 |
| pgAdmin | http://localhost:5050 |
| PostgreSQL | localhost:5432 |
| Redis | localhost:6379 |

pgAdmin: `admin@example.com` / `admin` — PostgreSQL: `postgres` / `postgres`

---

## Option C — Render (production/staging)

Services are deployed as Docker containers. Database is NeonDB (serverless PostgreSQL).

See [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) for full Render + NeonDB setup.

**Quick checklist:**
1. Create `coursedb` and `userdb` on NeonDB
2. Run EF Core migrations (see below)
3. Set environment variables in Render dashboard
4. Set Render **Root Directory to empty** and Dockerfile path to the full path from root

---

## Database Migrations (NeonDB)

Install the EF CLI tool once:
```bash
dotnet tool install --global dotnet-ef
```

Run from the **repo root**:

```bash
# CourseDB
dotnet ef database update \
  --project src/lms.shared.data \
  --startup-project src/lms.services/lms.services.coursemanagement \
  --context CourseDbContext \
  --connection "Host=ep-xxx.us-east-1.aws.neon.tech;Database=coursedb;Username=neondb_owner;Password=xxx;SSL Mode=Require;Trust Server Certificate=true;"

# UserDB
dotnet ef database update \
  --project src/lms.shared.data \
  --startup-project src/lms.services/lms.services.usermanagement \
  --context UserDbContext \
  --connection "Host=ep-xxx.us-east-1.aws.neon.tech;Database=userdb;Username=neondb_owner;Password=xxx;SSL Mode=Require;Trust Server Certificate=true;"
```

> Get your connection string from NeonDB dashboard → **Connection Details** → **.NET** tab.
> Convert from URI format to key-value format:
> `postgresql://user:pass@host/db` → `Host=host;Database=db;Username=user;Password=pass;SSL Mode=Require;Trust Server Certificate=true;`

Verify:
```sql
-- Run in NeonDB SQL Editor
SELECT * FROM "__EFMigrationsHistory";
```

---

## Environment Variables

### Render — Course Management

| Key | Value |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__CourseDatabase` | NeonDB key-value connection string for `coursedb` |
| `AWS__S3Bucket__Name` | S3 bucket name |
| `AWS__S3Bucket__Region` | e.g. `ap-south-1` |
| `AWS__SQS__QueueUrl` | SQS queue URL |

### Render — User Management

| Key | Value |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__UserDatabase` | NeonDB key-value connection string for `userdb` |

### Docker Compose — set in docker-compose.yml (already configured)

| Key | Value |
|---|---|
| `ConnectionStrings__CourseDatabase` | `Server=postgres;Port=5432;Database=coursedb;User Id=postgres;Password=postgres;` |
| `ConnectionStrings__UserDatabase` | `Server=postgres;Port=5432;Database=userdb;User Id=postgres;Password=postgres;` |
| `services__coursemanagement__http__0` | `http://coursemanagement:8080` |
| `services__usermanagement__http__0` | `http://usermanagement:8080` |

---

## Common Docker Commands

```bash
# View logs for all services
docker compose logs -f

# View logs for one service
docker compose logs -f coursemanagement

# Check service status
docker compose ps

# Shell into a container
docker compose exec coursemanagement bash

# Connect to local PostgreSQL
docker compose exec postgres psql -U postgres

# Create databases manually
docker compose exec postgres psql -U postgres -c "CREATE DATABASE coursedb;"
docker compose exec postgres psql -U postgres -c "CREATE DATABASE userdb;"
```

---

## Health Checks

```bash
curl http://localhost:8080/health   # coursemanagement
curl http://localhost:8081/health   # usermanagement
curl http://localhost:5173/health   # web
```

---

## API Testing

Via Swagger UI:
- http://localhost:8080/swagger (Course Management)
- http://localhost:8081/swagger (User Management)

Via curl:
```bash
curl http://localhost:8080/api/v1/courses
curl http://localhost:8080/health
```

Import into Postman: `http://localhost:8080/swagger/v1/swagger.json`

---

## Debugging

```bash
# Check which process uses a port (Windows)
netstat -ano | findstr :8080
taskkill /PID <pid> /F

# Check env vars inside a container
docker compose exec coursemanagement env | grep ConnectionStrings

# Check network between services
docker compose exec web ping coursemanagement
```

---

## Cleanup

```bash
# Remove all stopped containers, unused images, build cache
docker system prune -a

# Remove unused volumes only
docker volume prune
```
