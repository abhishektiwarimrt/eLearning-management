# Deployment Guide

## Overview

The eLearning Management System supports three deployment targets:

| Target | Database | Use case |
|---|---|---|
| Aspire (local) | Local PostgreSQL container | Development |
| Docker Compose (local) | Local PostgreSQL container | Integration testing |
| Render (cloud) | NeonDB (serverless PostgreSQL) | Production / staging |

---

## Architecture

```
Internet
    │
    ▼
lms.web (ASP.NET Core MVC)
    │
    ├── coursemanagement  (REST, port 8080)  → NeonDB: coursedb
    ├── usermanagement    (REST, port 8081)  → NeonDB: userdb
    └── discountgrpc      (gRPC, port 8083)  → SQLite (embedded)

Infrastructure:
    NeonDB (production)  or  PostgreSQL 18.1 (local Docker)
    Redis 7
```

---

## Prerequisites

- Docker Desktop 4.x+
- .NET 10 SDK (`global.json` locks to 10.0.201)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

---

## Project Structure

```
eLearning-management/
├── AppHost/                              # Aspire AppHost — dev only
│   ├── AppHost.csproj
│   ├── Program.cs
│   └── Properties/launchSettings.json
├── aspire/
│   └── ServiceDefaults/                  # Shared OpenTelemetry, health, discovery
├── src/
│   ├── lms.services/
│   │   ├── lms.services.coursemanagement/
│   │   │   ├── Dockerfile
│   │   │   ├── appsettings.json
│   │   │   └── appsettings.Production.json
│   │   ├── lms.services.usermanagement/
│   │   │   ├── Dockerfile
│   │   │   ├── appsettings.json
│   │   │   └── appsettings.Production.json
│   │   └── lms.services.discount.gRPC/Discount.gRPC/
│   │       └── Dockerfile
│   ├── lms.web/
│   │   └── Dockerfile
│   ├── lms.buildingblocks/
│   ├── lms.shared.common/
│   ├── lms.shared.data/                  # EF Core contexts + migrations
│   └── lms.services.aws/
├── docker-compose.yml
├── .dockerignore
├── global.json                           # Locks .NET SDK to 10.0.201
└── kubernetes-deployment.yaml
```

---

## NeonDB Setup (Production Database)

### 1. Create databases

In [NeonDB dashboard](https://console.neon.tech):
1. Create a project (e.g. `lms-production`)
2. Go to **Databases** → **New Database** → create `coursedb`
3. Go to **Databases** → **New Database** → create `userdb`

### 2. Get the connection string

In NeonDB dashboard → **Connection Details** → select **.NET** tab.

NeonDB gives a URI like:
```
postgresql://neondb_owner:abc123@ep-xxx.us-east-1.aws.neon.tech/coursedb?sslmode=require
```

**Convert to Npgsql key-value format** (required by .NET — do NOT use the URI format):
```
Host=ep-xxx.us-east-1.aws.neon.tech;Database=coursedb;Username=neondb_owner;Password=abc123;SSL Mode=Require;Trust Server Certificate=true;
```

Mapping from URI to key-value:
```
postgresql:// neondb_owner : abc123 @ ep-xxx.us-east-1.aws.neon.tech / coursedb
              ^^^^^^^^^^^^   ^^^^^^   ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^  ^^^^^^^^
              Username       Password Host                                Database
```

### 3. Run EF Core migrations against NeonDB

Run these from the **repo root**:

**CourseDB:**
```bash
dotnet ef database update \
  --project src/lms.shared.data \
  --startup-project src/lms.services/lms.services.coursemanagement \
  --context CourseDbContext \
  --connection "Host=ep-xxx.us-east-1.aws.neon.tech;Database=coursedb;Username=neondb_owner;Password=abc123;SSL Mode=Require;Trust Server Certificate=true;"
```

**UserDB:**
```bash
dotnet ef database update \
  --project src/lms.shared.data \
  --startup-project src/lms.services/lms.services.usermanagement \
  --context UserDbContext \
  --connection "Host=ep-xxx.us-east-1.aws.neon.tech;Database=userdb;Username=neondb_owner;Password=abc123;SSL Mode=Require;Trust Server Certificate=true;"
```

Verify migrations applied (run in NeonDB SQL Editor):
```sql
SELECT * FROM "__EFMigrationsHistory";
```

### 4. Adding a new migration

```bash
# CourseDB
dotnet ef migrations add <MigrationName> \
  --project src/lms.shared.data \
  --startup-project src/lms.services/lms.services.coursemanagement \
  --context CourseDbContext \
  --output-dir Migrations/CourseDb

# UserDB
dotnet ef migrations add <MigrationName> \
  --project src/lms.shared.data \
  --startup-project src/lms.services/lms.services.usermanagement \
  --context UserDbContext \
  --output-dir Migrations
```

---

## Render Deployment

### Render service settings

For each service, set in Render dashboard → **Settings**:

| Setting | Course Management | User Management |
|---|---|---|
| **Root Directory** | *(leave empty)* | *(leave empty)* |
| **Dockerfile Path** | `src/lms.services/lms.services.coursemanagement/Dockerfile` | `src/lms.services/lms.services.usermanagement/Dockerfile` |
| **Docker Build Context** | repo root | repo root |

> Root Directory must be **empty** (not the service subfolder) so Docker can reach `aspire/ServiceDefaults/` during the build.

### Environment variables (Render)

Set these in **Render dashboard → your service → Environment**:

**Course Management:**

| Key | Value |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__CourseDatabase` | `Host=ep-xxx...neon.tech;Database=coursedb;Username=neondb_owner;Password=xxx;SSL Mode=Require;Trust Server Certificate=true;` |
| `AWS__S3Bucket__Name` | your S3 bucket name |
| `AWS__S3Bucket__Region` | e.g. `ap-south-1` |
| `AWS__SQS__QueueUrl` | your SQS queue URL |

**User Management:**

| Key | Value |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__UserDatabase` | `Host=ep-xxx...neon.tech;Database=userdb;Username=neondb_owner;Password=xxx;SSL Mode=Require;Trust Server Certificate=true;` |

> Double underscore `__` is the .NET config separator for nested keys.
> These env vars override anything in `appsettings.Production.json` at runtime.

### First deploy / cache issues

If you see a Npgsql version mismatch error on Render:
1. Render dashboard → service → **Manual Deploy**
2. Enable **Clear build cache & deploy**

This forces `dotnet restore` to run fresh and pull the correct package versions.

---

## Development with Aspire

```bash
cd AppHost
dotnet run --launch-profile https
```

Dashboard: `https://localhost:15889`

Aspire automatically:
- Starts Redis as a Docker container
- Injects `services__<name>__https__0` env vars for service-to-service discovery
- Exports OpenTelemetry (logs, traces, metrics) to the dashboard

For local development, connect to a local PostgreSQL or a NeonDB dev branch.

---

## Docker Compose (local full stack)

```bash
# Build all images and start
docker compose up --build

# Start in background
docker compose up -d

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

pgAdmin: `admin@example.com` / `admin`
PostgreSQL: `postgres` / `postgres`

### Service discovery in Docker Compose

Set as environment variables in `docker-compose.yml` (matching the Aspire-injected key format):
```yaml
web:
  environment:
    - services__coursemanagement__http__0=http://coursemanagement:8080
    - services__usermanagement__http__0=http://usermanagement:8080
    - services__discountgrpc__http__0=http://discountgrpc:8080
```

---

## Building Docker Images Manually

All Dockerfiles **must be built from the repo root** as context (so `aspire/ServiceDefaults/` is reachable):

```bash
docker build -f src/lms.services/lms.services.coursemanagement/Dockerfile -t lms-coursemanagement:latest .
docker build -f src/lms.services/lms.services.usermanagement/Dockerfile -t lms-usermanagement:latest .
docker build -f src/lms.services/lms.services.discount.gRPC/Discount.gRPC/Dockerfile -t lms-discountgrpc:latest .
docker build -f src/lms.web/Dockerfile -t lms-web:latest .
```

---

## Environment Variables Reference

### How .NET config precedence works

```
appsettings.json
    ↓ overridden by
appsettings.Production.json
    ↓ overridden by
Environment variables  ← highest priority (set these in Render / Docker)
```

`appsettings.Production.json` files contain placeholder values (`#{...}#`) — they are always overridden by environment variables and are never used directly.

### All variables

| Variable | Service | Example Value |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | All | `Production` |
| `ASPNETCORE_URLS` | All | `http://+:8080` |
| `ConnectionStrings__CourseDatabase` | coursemanagement | NeonDB connection string |
| `ConnectionStrings__UserDatabase` | usermanagement | NeonDB connection string |
| `ConnectionStrings__Redis` | coursemanagement, usermanagement | `redis:6379` |
| `AWS__S3Bucket__Name` | coursemanagement | `my-lms-bucket` |
| `AWS__S3Bucket__Region` | coursemanagement | `ap-south-1` |
| `AWS__SQS__QueueUrl` | coursemanagement | `https://sqs...amazonaws.com/...` |
| `services__coursemanagement__http__0` | web | `http://coursemanagement:8080` |
| `services__usermanagement__http__0` | web | `http://usermanagement:8080` |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | All (optional) | `http://otelcollector:4317` |

---

## Kubernetes Deployment

```bash
kubectl apply -f kubernetes-deployment.yaml
kubectl get pods -n lms
kubectl get svc -n lms
kubectl logs -n lms deployment/coursemanagement
kubectl port-forward -n lms svc/web 5173:80
```

Update image names in `kubernetes-deployment.yaml` before applying:
```yaml
image: your-registry.azurecr.io/lms/coursemanagement:latest
```

Set NeonDB connection strings as Kubernetes secrets:
```bash
kubectl create secret generic db-secrets -n lms \
  --from-literal=course-db="Host=ep-xxx...neon.tech;Database=coursedb;Username=neondb_owner;Password=xxx;SSL Mode=Require;Trust Server Certificate=true;" \
  --from-literal=user-db="Host=ep-xxx...neon.tech;Database=userdb;Username=neondb_owner;Password=xxx;SSL Mode=Require;Trust Server Certificate=true;"
```

---

## Troubleshooting

### Render: "not found" error for aspire/ServiceDefaults
Root Directory in Render is set to the service subfolder instead of the repo root.
Fix: Clear the Root Directory field so Render uses the repo root as Docker context.

### Render: Npgsql version mismatch (get_LockReleaseBehavior)
Stale build cache is loading an old Npgsql version.
Fix: Render dashboard → Manual Deploy → **Clear build cache & deploy**.

### dotnet ef: "Unable to retrieve project metadata"
Framework mismatch — a dependency targets a different .NET version than the startup project.
Fix: Ensure `lms.shared.data`, `lms.shared.common`, `lms.services.aws` all target `net10.0`.

### NeonDB: connection string format error
Using URI format (`postgresql://...`) instead of key-value format.
Fix: Use `Host=...;Database=...;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true;`

### Port already in use
```bash
netstat -ano | findstr :8080
taskkill /PID <pid> /F
```

### Services can't reach each other (Docker Compose)
```bash
docker compose exec web ping coursemanagement
docker compose exec web env | grep services__
```

### Out of disk space
```bash
docker system prune -a --volumes
```

---

## Production Checklist

- [ ] NeonDB databases `coursedb` and `userdb` created
- [ ] EF Core migrations applied to both NeonDB databases
- [ ] `ASPNETCORE_ENVIRONMENT=Production` set in Render
- [ ] NeonDB connection strings set in Render env vars (key-value format)
- [ ] AWS credentials set if using S3/SQS
- [ ] Render Root Directory left empty (repo root as Docker context)
- [ ] First deploy uses "Clear build cache"
- [ ] Health endpoints responding: `/health`, `/alive`
- [ ] `global.json` committed (locks SDK to 10.0.201)
