# eLearning Management System

A microservices-based LMS built with .NET 10, .NET Aspire 13.2.2, and Docker.

## Architecture

```
┌──────────────────────────────────────────────────┐
│               Web Application (lms.web)           │
└──────────────┬───────────────────────────────────┘
               │
   ┌───────────┼───────────┬──────────────┐
   ▼           ▼           ▼              ▼
CourseManagement  UserManagement  Discount gRPC
(REST API)        (REST API)      (gRPC)
   │           │
   └─────┬─────┘
         ▼
    PostgreSQL
    Redis Cache
```

## Services

| Service | Dev Port | Protocol | Database |
|---|---|---|---|
| Course Management | 8080 | REST | coursedb (PostgreSQL) |
| User Management | 8081 | REST | userdb (PostgreSQL) |
| Discount gRPC | 8083 | gRPC | SQLite |
| Web App | 5173 | HTTP | — |
| PostgreSQL | 5432 | — | — |
| Redis | 6379 | — | — |
| pgAdmin | 5050 | HTTP | — |

## Running the System

### Option A — Aspire (recommended for development)

```bash
cd AppHost
dotnet run --launch-profile https
```

Aspire Dashboard: `https://localhost:15889`  
All services are launched automatically with service discovery wired up.

### Option B — Docker Compose

```bash
docker compose up --build
```

| URL | Description |
|---|---|
| http://localhost:5173 | Web application |
| http://localhost:8080/swagger | Course Management API |
| http://localhost:8081/swagger | User Management API |
| http://localhost:8083 | Discount gRPC |
| http://localhost:5050 | pgAdmin (admin@example.com / admin) |

## Project Structure

```
eLearning-management/
├── AppHost/                          # .NET Aspire AppHost (dev orchestration)
│   ├── AppHost.csproj
│   └── Program.cs
├── aspire/
│   └── ServiceDefaults/              # Shared OpenTelemetry, health checks, service discovery
│       ├── ServiceDefaults.csproj
│       └── Extensions.cs
├── src/
│   ├── lms.services/
│   │   ├── lms.services.coursemanagement/   # Course REST API
│   │   ├── lms.services.usermanagement/     # User REST API
│   │   └── lms.services.discount.gRPC/      # Discount gRPC service
│   │       └── Discount.gRPC/
│   ├── lms.services.instructor/             # Instructor service (in development)
│   ├── lms.web/                             # ASP.NET Core MVC web frontend
│   ├── lms.buildingblocks/                  # Carter, versioned API, Swagger helpers
│   ├── lms.shared.common/                   # Shared DTOs and models
│   ├── lms.shared.data/                     # EF Core contexts and migrations
│   └── lms.services.aws/                    # AWS S3/SQS integration
├── docker-compose.yml
├── .dockerignore
├── kubernetes-deployment.yaml
└── README.md
```

## Database Migrations

**Course Management**
```bash
Add-Migration InitialCreate -Project lms.shared.data -StartupProject lms.services.coursemanagement -Context CourseDbContext
```

**User Management**
```bash
Add-Migration InitialCreate -Project lms.shared.data -StartupProject lms.services.usermanagement -Context UserDbContext
```

## Technology Stack

- **.NET 10** — runtime
- **.NET Aspire 13.2.2** — dev orchestration, OpenTelemetry, service discovery
- **Carter** — minimal API endpoint mapping
- **MediatR** — command/query handler pattern
- **FluentValidation** — request validation
- **Entity Framework Core 10** — ORM (PostgreSQL / SQLite)
- **Serilog** — structured logging
- **Swashbuckle 10** — Swagger / OpenAPI
- **gRPC (Grpc.AspNetCore)** — Discount service protocol
- **Redis (StackExchange.Redis)** — distributed cache
- **Docker / Docker Compose** — containerization
