# Deployment Guide

## Overview

The eLearning Management System is a .NET 10 microservices application. It has two deployment modes:

- **Development** — .NET Aspire orchestrates all services locally with automatic service discovery, OpenTelemetry, and a built-in dashboard
- **Production** — Docker Compose (or Kubernetes) runs each service as an independent container; Aspire is not used

## Architecture

```
Internet
    │
    ▼
lms.web (ASP.NET Core MVC)
    │
    ├── coursemanagement  (REST, port 8080)  → PostgreSQL (coursedb)
    ├── usermanagement    (REST, port 8081)  → PostgreSQL (userdb)
    └── discountgrpc      (gRPC, port 8083)  → SQLite
    
Infrastructure:
    PostgreSQL 18        (port 5432)
    Redis 7              (port 6379)
    pgAdmin              (port 5050)
```

## Prerequisites

- Docker Desktop 4.x+
- .NET 10 SDK (for development / Aspire)
- Git

## Project Structure

```
eLearning-management/
├── AppHost/                              # Aspire AppHost — dev only
│   ├── AppHost.csproj
│   ├── Program.cs
│   └── Properties/launchSettings.json
├── aspire/
│   └── ServiceDefaults/                  # Shared defaults library
│       ├── ServiceDefaults.csproj
│       └── Extensions.cs
├── src/
│   ├── lms.services/
│   │   ├── lms.services.coursemanagement/
│   │   │   └── Dockerfile
│   │   ├── lms.services.usermanagement/
│   │   │   └── Dockerfile
│   │   └── lms.services.discount.gRPC/
│   │       └── Discount.gRPC/
│   │           └── Dockerfile
│   ├── lms.web/
│   │   └── Dockerfile
│   ├── lms.buildingblocks/
│   ├── lms.shared.common/
│   ├── lms.shared.data/
│   └── lms.services.aws/
├── docker-compose.yml
├── .dockerignore
└── kubernetes-deployment.yaml
```

## Development with Aspire

Aspire handles service discovery, environment variable injection, container lifecycle (Redis), OpenTelemetry, and the monitoring dashboard.

```bash
cd AppHost
dotnet run --launch-profile https
```

Dashboard: `https://localhost:15889`

Aspire injects `services__<name>__https__0` and `services__<name>__http__0` environment variables into each service so they can find each other without hardcoded URLs.

### How service URLs are resolved

In `AppHost/Program.cs`:
```csharp
var courseManagementService = builder.AddProject<Projects.lms_services_coursemanagement>("coursemanagement", launchProfileName: "https");

builder.AddProject<Projects.lms_web>("web", launchProfileName: "https")
    .WithReference(courseManagementService);   // injects services__coursemanagement__https__0
```

In `lms.web/Services/UserManagementService.cs` the service reads:
```csharp
config["services__usermanagement__https__0"] ??
config["services__usermanagement__http__0"] ??
config["MicroServices:UserManagementUrl"]
```

## Docker Compose Deployment

All Dockerfiles are built from the **repo root** context so they can reach `aspire/ServiceDefaults/`.

```bash
# Build all images and start
docker compose up --build

# Start without rebuilding
docker compose up

# Start in background
docker compose up -d

# Stop
docker compose down

# Stop and delete volumes (resets databases)
docker compose down -v
```

### Service endpoints

| Service | Host Port | Container Port | URL |
|---|---|---|---|
| coursemanagement | 8080 | 8080 | http://localhost:8080/swagger |
| usermanagement | 8081 | 8080 | http://localhost:8081/swagger |
| discountgrpc | 8083 | 8080 | http://localhost:8083 |
| web | 5173 | 8080 | http://localhost:5173 |
| postgres | 5432 | 5432 | localhost:5432 |
| redis | 6379 | 6379 | localhost:6379 |
| pgadmin | 5050 | 80 | http://localhost:5050 |

### Service discovery in Docker Compose

Since Aspire is not running, service URLs are set as environment variables in `docker-compose.yml`:

```yaml
web:
  environment:
    - services__usermanagement__http__0=http://usermanagement:8080
    - services__coursemanagement__http__0=http://coursemanagement:8080
    - services__discountgrpc__http__0=http://discountgrpc:8080
```

These match exactly what the services read in code (the same `services__*__http__0` keys that Aspire injects during development).

### Database setup

PostgreSQL databases are created automatically by EF Core migrations on first run. To create them manually via pgAdmin:

1. Open http://localhost:5050
2. Login: `admin@example.com` / `admin`
3. Connect to server: host=`postgres`, user=`postgres`, password=`postgres`
4. Create databases: `coursedb`, `userdb`

Or via CLI:
```bash
docker compose exec postgres psql -U postgres -c "CREATE DATABASE coursedb;"
docker compose exec postgres psql -U postgres -c "CREATE DATABASE userdb;"
```

## Building Individual Docker Images

All Dockerfiles must be run from the **repo root** as build context:

```bash
# Course Management
docker build -f src/lms.services/lms.services.coursemanagement/Dockerfile -t lms-coursemanagement:latest .

# User Management
docker build -f src/lms.services/lms.services.usermanagement/Dockerfile -t lms-usermanagement:latest .

# Discount gRPC
docker build -f src/lms.services/lms.services.discount.gRPC/Discount.gRPC/Dockerfile -t lms-discountgrpc:latest .

# Web
docker build -f src/lms.web/Dockerfile -t lms-web:latest .
```

## Kubernetes Deployment

```bash
# Apply full manifest
kubectl apply -f kubernetes-deployment.yaml

# Check status
kubectl get pods -n lms
kubectl get svc -n lms

# View logs
kubectl logs -n lms deployment/coursemanagement

# Port-forward web app locally
kubectl port-forward -n lms svc/web 5173:80
```

Update image names in `kubernetes-deployment.yaml` before applying:
```yaml
image: your-registry.azurecr.io/lms/coursemanagement:latest
```

## Environment Variables Reference

### All services

| Variable | Description |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Development` or `Production` |
| `ASPNETCORE_URLS` | e.g. `http://+:8080` |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | OTLP collector endpoint (optional) |

### Course Management

| Variable | Example |
|---|---|
| `ConnectionStrings__CourseDatabase` | `Server=postgres;Port=5432;Database=coursedb;User Id=postgres;Password=postgres;` |
| `ConnectionStrings__Redis` | `redis:6379` |

### User Management

| Variable | Example |
|---|---|
| `ConnectionStrings__UserDatabase` | `Server=postgres;Port=5432;Database=userdb;User Id=postgres;Password=postgres;` |
| `ConnectionStrings__Redis` | `redis:6379` |

### Web App

| Variable | Example |
|---|---|
| `services__coursemanagement__http__0` | `http://coursemanagement:8080` |
| `services__usermanagement__http__0` | `http://usermanagement:8080` |
| `services__discountgrpc__http__0` | `http://discountgrpc:8080` |

## Monitoring and Telemetry

### Development (Aspire Dashboard)

The Aspire Dashboard at `https://localhost:15889` provides:
- Structured logs from all services
- Distributed traces (request flows across services)
- Metrics (CPU, memory, request counts, latency)
- Health status

OpenTelemetry is wired up via `ServiceDefaults/Extensions.cs`. The `AddServiceDefaults()` call in each service's `Program.cs` enables it automatically when the `OTEL_EXPORTER_OTLP_ENDPOINT` env var is set (Aspire sets this).

### Production (OTEL Collector)

Add an OpenTelemetry Collector to `docker-compose.yml` and set on each service:

```yaml
environment:
  - OTEL_EXPORTER_OTLP_ENDPOINT=http://otelcollector:4317
```

`ServiceDefaults` already checks for this env var and activates the OTLP exporter automatically.

## Scaling

```bash
# Scale course management to 3 replicas
docker compose up -d --scale coursemanagement=3
```

For Kubernetes, configure HPA (Horizontal Pod Autoscaler) targeting CPU utilization.

## Troubleshooting

### Port already in use
```bash
# Find the process (Windows)
netstat -ano | findstr :8080

# Kill it
taskkill /PID <pid> /F
```

### Service can't reach another service
```bash
# Check Docker network
docker network ls
docker compose exec web ping coursemanagement

# Verify env vars are set
docker compose exec web env | grep services__
```

### Database connection fails
```bash
# Check postgres is healthy
docker compose ps postgres
docker compose logs postgres

# Verify connection string in container
docker compose exec coursemanagement env | grep ConnectionStrings
```

### Proto file not found (gRPC build error on Linux)
This happens due to case-sensitivity: `Protos\discount.proto` vs `Protos\Discount.proto`. The `.csproj` must match the exact filename case. Fixed in `Discount.gRPC.csproj` to use `Protos\Discount.proto`.

### Docker build context errors (missing shared projects)
All Dockerfiles use `.` (repo root) as build context. If you build a Dockerfile directly, always run from the repo root:
```bash
docker build -f src/lms.services/lms.services.coursemanagement/Dockerfile .
```

### Out of disk space
```bash
docker system prune -a --volumes
```

## Production Checklist

- [ ] Change all default passwords (PostgreSQL, pgAdmin)
- [ ] Use secrets management (Kubernetes Secrets, Azure Key Vault, or Docker secrets)
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Configure TLS termination at load balancer or Nginx
- [ ] Set resource limits in docker-compose or Kubernetes manifests
- [ ] Configure backup strategy for PostgreSQL volumes
- [ ] Point `OTEL_EXPORTER_OTLP_ENDPOINT` to a collector (Prometheus, Grafana, Seq, etc.)
- [ ] Update image registry URLs in kubernetes-deployment.yaml
- [ ] Configure CI/CD pipeline (.github/workflows/docker-build.yml)
