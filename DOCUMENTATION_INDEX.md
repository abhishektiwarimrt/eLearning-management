# Documentation Index

## Start here

| Goal | Document |
|---|---|
| Run the system immediately | [QUICK_START.md](QUICK_START.md) |
| Deploy to Render + NeonDB | [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) |
| Understand the full architecture | [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) |
| Understand Aspire setup | [ASPIRE_SETUP_SUMMARY.md](ASPIRE_SETUP_SUMMARY.md) |
| Project overview | [README.md](README.md) |

---

## By task

### Run locally with Docker Compose
→ [QUICK_START.md](QUICK_START.md) — `docker compose up --build`

### Develop with Aspire
→ [ASPIRE_SETUP_SUMMARY.md](ASPIRE_SETUP_SUMMARY.md) — `cd AppHost && dotnet run --launch-profile https`

### Deploy to Render
→ [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md#render-deployment) — Root Directory, Dockerfile path, env vars

### Set up NeonDB and run migrations
→ [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md#neondb-setup-production-database) — create databases, convert connection string, run `dotnet ef database update`

### Set environment variables
→ [QUICK_START.md](QUICK_START.md#environment-variables) — table of all keys per service

### Debug a failing service
→ [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md#troubleshooting) — common errors and fixes

### Add a new EF Core migration
→ [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md#adding-a-new-migration)

---

## Service Endpoints

### Docker Compose (local)

| Service | URL |
|---|---|
| Web App | http://localhost:5173 |
| Course Management API | http://localhost:8080/swagger |
| User Management API | http://localhost:8081/swagger |
| Discount gRPC | http://localhost:8083 |
| pgAdmin | http://localhost:5050 |
| PostgreSQL | localhost:5432 |
| Redis | localhost:6379 |

### Aspire (local development)

| Resource | URL |
|---|---|
| Aspire Dashboard | https://localhost:15889 |
| All services | Managed by Aspire (ports shown in dashboard) |

### Render (production)

| Service | URL |
|---|---|
| Course Management | `https://<render-service>.onrender.com` |
| User Management | `https://<render-service>.onrender.com` |

---

## Database

| Environment | Provider | Databases |
|---|---|---|
| Local (Docker Compose) | PostgreSQL 18.1 container | `coursedb`, `userdb` |
| Local (Aspire) | PostgreSQL container (via Aspire) | `coursedb`, `userdb` |
| Production (Render) | NeonDB (serverless PostgreSQL) | `coursedb`, `userdb` |

**NeonDB connection string format** (Npgsql key-value — always use this format):
```
Host=ep-xxx.us-east-1.aws.neon.tech;Database=<db>;Username=neondb_owner;Password=<pass>;SSL Mode=Require;Trust Server Certificate=true;
```

**Migration commands** (run from repo root):
```bash
dotnet ef database update \
  --project src/lms.shared.data \
  --startup-project src/lms.services/lms.services.coursemanagement \
  --context CourseDbContext \
  --connection "<NeonDB connection string for coursedb>"

dotnet ef database update \
  --project src/lms.shared.data \
  --startup-project src/lms.services/lms.services.usermanagement \
  --context UserDbContext \
  --connection "<NeonDB connection string for userdb>"
```

---

## Key Configuration Files

| File | Purpose |
|---|---|
| `docker-compose.yml` | Full local stack |
| `global.json` | Locks .NET SDK to 10.0.201 |
| `AppHost/Program.cs` | Aspire orchestration |
| `AppHost/Properties/launchSettings.json` | Aspire dashboard / OTLP URLs |
| `aspire/ServiceDefaults/Extensions.cs` | OpenTelemetry, health checks, service discovery |
| `src/lms.shared.data/Migrations/` | EF Core migration files |
| `kubernetes-deployment.yaml` | Kubernetes manifests |
| `.dockerignore` | Docker build exclusions |

---

## Technology Stack

| Technology | Version | Role |
|---|---|---|
| .NET | 10 (SDK 10.0.201) | Runtime |
| .NET Aspire | 13.2.2 | Dev orchestration |
| NeonDB | — | Production PostgreSQL (serverless) |
| PostgreSQL | 18.1 | Local dev database |
| Redis | 7 | Distributed cache |
| EF Core | 10.0.5 | ORM |
| Npgsql EF Provider | 10.0.1 | PostgreSQL EF driver |
| Carter | 10.0 | Minimal API routing |
| MediatR | 14.1 | CQRS handlers |
| FluentValidation | 12.1 | Request validation |
| Swashbuckle | 10.1 | Swagger / OpenAPI |
| Grpc.AspNetCore | 2.67 | gRPC (Discount service) |
| Serilog | 4.3 | Structured logging |
| OpenTelemetry | 1.14 | Metrics, traces, logs |
| Docker | — | Containerization |
| Render | — | Cloud deployment platform |
