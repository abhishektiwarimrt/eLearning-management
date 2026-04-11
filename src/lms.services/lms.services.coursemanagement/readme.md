# lms.services.coursemanagement

Course Management microservice for the LMS platform. Built on .NET 10 ASP.NET Core using the minimal API pattern with Carter, MediatR, and Entity Framework Core.

## Responsibilities

- Course CRUD (create, read, update, delete)
- Course section management (bulk creation within a transaction)
- Course module management (file upload support via multipart/form-data)
- Course enrollment abstraction
- Background file upload processing (AWS S3 via `lms.services.aws`)

## Architecture

```
HTTP Request
    │
    ▼
Carter Endpoint (Features/V1/)
    │
    ▼
MediatR Command Handler
    │
    ├── Application Service (ICourseService, ICourseSectionService, ...)
    │       └── IUnitOfWork (EF Core transaction boundary)
    └── ILogger<T>
```

### Key layers

| Layer | Location | Description |
|---|---|---|
| Endpoints | `Features/V1/` | Carter `ICarterModule` classes — HTTP mapping only |
| Handlers | `Features/V1/` | MediatR handlers — orchestrate services + UoW |
| Services | `Services/` | Business logic (`CourseService`, `CourseSectionService`, …) |
| Background | `Background/` | `FileUploadWorkerService` — hosted service for async file uploads |
| Exceptions | `Exceptions/` | `CourseException`, `CourseSectionException` |

## Project Structure

```
lms.services.coursemanagement/
├── Program.cs
├── Dockerfile
├── Features/
│   └── V1/
│       ├── Course/
│       │   ├── CreateCourseEndpoint.cs
│       │   └── CreateCourseHandler.cs
│       ├── CourseSection/
│       │   ├── CreateCourseSectionEndpoint.cs
│       │   └── CreateCourseSectionHandler.cs
│       └── CourseModule/
│           ├── AddCourseModuleEndpoint.cs
│           └── AddCourseModuleHandler.cs
├── Services/
│   ├── ICourseService.cs / CourseService.cs
│   ├── ICourseSectionService.cs / CourseSectionService.cs
│   ├── ICourseModuleService.cs / CourseModuleService.cs
│   └── ICourseEnrollmentService.cs / CourseEnrollmentService.cs
├── Background/
│   └── FileUploadWorkerService.cs
└── Exceptions/
    ├── CourseException.cs
    └── CourseSectionException.cs
```

## API Endpoints

Base path: `/api/v1`

### POST /api/v1/courses
Create a new course with optional initial sections.

Request body:
```json
{
  "courseDto": {
    "title": "string",
    "description": "string",
    "session": 0,
    "sections": [
      { "title": "string", "description": "string", "order": 0 }
    ]
  }
}
```

Response `201`:
```json
{ "status": "string", "data": { "created": true }, "version": "string" }
```

### POST /api/v1/courses/{courseId}/sections
Create one or more sections for an existing course (transactional).

Request body:
```json
{
  "courseSectionDtos": [
    { "title": "string", "description": "string", "order": 0 }
  ]
}
```

Response `201`: `{ "courseSectionsCreated": true }`

### POST /api/v1/courses/{courseId}/sections/{sectionId}/modules
Create modules for a section. Accepts `multipart/form-data`.

Request body:
```json
{
  "courseModules": [
    { "title": "string", "contentType": "string", "content": "string", "order": 0, "fileUploaded": true }
  ]
}
```

Response `201`: envelope with `data.courseModules` array.

### Additional operations (via ICourseService)

- `GET /api/v1/courses` — list all courses
- `GET /api/v1/courses/{id}` — get course by ID
- `PUT /api/v1/courses/{id}` — update course
- `DELETE /api/v1/courses/{id}` — delete course

## Running locally

### With Aspire (recommended)
```bash
cd AppHost
dotnet run --launch-profile https
```
Aspire injects the database connection string and starts all dependencies.

### Standalone
```bash
cd src/lms.services/lms.services.coursemanagement
dotnet run
```
Requires a PostgreSQL instance and `ConnectionStrings__CourseDatabase` in appsettings or environment.

### With Docker Compose
```bash
# From repo root
docker compose up --build coursemanagement
```

Service is available at http://localhost:8080/swagger

## Database

Uses EF Core with PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL`).

```bash
# Create initial migration (from solution root)
Add-Migration InitialCreate \
  -Project lms.shared.data \
  -StartupProject lms.services.coursemanagement \
  -Context CourseDbContext
```

Connection string format:
```
Server=<host>;Port=5432;Database=coursedb;User Id=postgres;Password=postgres;
```

## Dependencies

| Project | Purpose |
|---|---|
| `lms.buildingblocks` | Carter, MediatR, Swagger, versioned API setup |
| `lms.shared.common` | Shared DTOs and response envelopes |
| `lms.shared.data` | EF Core `CourseDbContext`, `IUnitOfWork` |
| `lms.services.aws` | S3 file upload integration |
| `aspire/ServiceDefaults` | OpenTelemetry, health checks, service discovery |

## Technology Stack

- .NET 10 ASP.NET Core
- Carter 10 — minimal API routing
- MediatR 14 — command/query handlers
- FluentValidation 12 — request validation
- EF Core 10 + Npgsql — data access
- Swashbuckle 10 — Swagger / OpenAPI
- Serilog 4 — structured logging
- OpenTelemetry 1.14 — metrics, traces, logs
