# Documentation Index

## Start here

| Goal | Document |
|---|---|
| Run the system immediately | [QUICK_START.md](QUICK_START.md) |
| Understand the full architecture | [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) |
| Understand Aspire setup | [ASPIRE_SETUP_SUMMARY.md](ASPIRE_SETUP_SUMMARY.md) |
| Understand the project overall | [README.md](README.md) |

## By task

### I want to run locally with Docker
→ [QUICK_START.md](QUICK_START.md) — `docker compose up --build`

### I want to develop with Aspire
→ [ASPIRE_SETUP_SUMMARY.md](ASPIRE_SETUP_SUMMARY.md) — `cd AppHost && dotnet run --launch-profile https`

### I want to deploy to production
→ [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) — Docker Compose or Kubernetes sections

### I want to debug a failing service
→ [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md#troubleshooting) — Troubleshooting section

### I want to understand service discovery
→ [ASPIRE_SETUP_SUMMARY.md](ASPIRE_SETUP_SUMMARY.md) — explains `services__*` env vars in both modes

## Service Endpoints

### Docker Compose

| Service | URL |
|---|---|
| Web App | http://localhost:5173 |
| Course Management API | http://localhost:8080/swagger |
| User Management API | http://localhost:8081/swagger |
| Discount gRPC | http://localhost:8083 |
| pgAdmin | http://localhost:5050 |
| PostgreSQL | localhost:5432 |
| Redis | localhost:6379 |

### Aspire (development)

| Resource | URL |
|---|---|
| Aspire Dashboard | https://localhost:15889 |
| All services | Managed by Aspire (ports shown in dashboard) |

## Key configuration files

| File | Purpose |
|---|---|
| `docker-compose.yml` | Full stack for Docker deployment |
| `AppHost/Program.cs` | Aspire orchestration |
| `AppHost/Properties/launchSettings.json` | Aspire dashboard and OTLP URLs |
| `aspire/ServiceDefaults/Extensions.cs` | OpenTelemetry, health checks, service discovery |
| `kubernetes-deployment.yaml` | Kubernetes manifests |
| `.dockerignore` | Docker build exclusions |

## Technology Stack

| Technology | Version | Role |
|---|---|---|
| .NET | 10 | Runtime |
| .NET Aspire | 13.2.2 | Dev orchestration |
| PostgreSQL | 18.1 | Primary database |
| Redis | 7 | Distributed cache |
| Carter | 10.0 | Minimal API routing |
| MediatR | 14.1 | CQRS handlers |
| FluentValidation | 12.1 | Request validation |
| EF Core | 10.0 | ORM |
| Swashbuckle | 10.1 | Swagger / OpenAPI |
| Grpc.AspNetCore | 2.67 | gRPC (Discount service) |
| Serilog | 4.3 | Structured logging |
| OpenTelemetry | 1.14 | Metrics, traces, logs |
| Docker | — | Containerization |
