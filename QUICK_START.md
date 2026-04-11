# Quick Start Guide

## Development with Aspire (recommended)

```bash
cd AppHost
dotnet run --launch-profile https
```

Open the Aspire Dashboard at `https://localhost:15889` — it shows all running services, logs, traces, and metrics.

> Prerequisites: .NET 10 SDK, Docker Desktop (for Redis container)

## Docker Compose (full stack)

```bash
# From repo root — builds all images and starts everything
docker compose up --build

# Rebuild a single service after code changes
docker compose up --build coursemanagement

# Stop and remove containers
docker compose down

# Stop and remove containers + volumes (clears database data)
docker compose down -v
```

### Service URLs (Docker Compose)

| Service | URL |
|---|---|
| Web App | http://localhost:5173 |
| Course Management API | http://localhost:8080/swagger |
| User Management API | http://localhost:8081/swagger |
| Discount gRPC | http://localhost:8083 |
| pgAdmin | http://localhost:5050 |
| PostgreSQL | localhost:5432 |
| Redis | localhost:6379 |

pgAdmin login: `admin@example.com` / `admin`  
PostgreSQL: `postgres` / `postgres`

## Common Docker Commands

```bash
# View logs for all services
docker compose logs -f

# View logs for a specific service
docker compose logs -f coursemanagement

# Check service status
docker compose ps

# Open a shell in a container
docker compose exec coursemanagement bash

# Connect to PostgreSQL
docker compose exec postgres psql -U postgres

# Create databases manually (EF migrations do this automatically)
docker compose exec postgres psql -U postgres -c "CREATE DATABASE coursedb;"
docker compose exec postgres psql -U postgres -c "CREATE DATABASE userdb;"
```

## Health Checks

All services expose `/health` and `/alive` endpoints:

```bash
curl http://localhost:8080/health
curl http://localhost:8081/health
curl http://localhost:5173/health
```

## API Testing

### Via Swagger UI
- Course Management: http://localhost:8080/swagger
- User Management: http://localhost:8081/swagger

### Via curl
```bash
# Get all courses
curl http://localhost:8080/api/v1/courses

# Health check
curl http://localhost:8080/health
```

### Via Postman
Import from: `http://localhost:8080/swagger/v1/swagger.json`

## Debugging

```bash
# Check which process is using a port (Windows)
netstat -ano | findstr :8080

# Verify environment variables inside a container
docker compose exec coursemanagement env | grep ConnectionStrings

# Check network connectivity between services
docker compose exec web ping coursemanagement
```

## Rebuilding After Code Changes

```bash
# Rebuild and restart a single service
docker compose up --build coursemanagement

# Rebuild all services from scratch (no cache)
docker compose build --no-cache
docker compose up
```

## Cleanup

```bash
# Remove all stopped containers, unused images, build cache
docker system prune -a

# Remove only unused volumes
docker volume prune
```
