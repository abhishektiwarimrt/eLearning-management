# .NET Aspire Setup Summary

## What is Aspire used for

.NET Aspire 13.2.2 is the **development orchestrator** for this project. It:

- Starts all microservices and the web frontend in one command
- Spins up Redis as a Docker container automatically
- Injects `services__<name>__https__0` / `services__<name>__http__0` env vars so services find each other without hardcoded URLs
- Exports OpenTelemetry data (logs, traces, metrics) to the built-in dashboard
- Provides health check endpoints (`/health`, `/alive`) on every service

Aspire is **not used in production Docker or Kubernetes** — only on developer machines.

## Key Files

### AppHost

| File | Purpose |
|---|---|
| `AppHost/AppHost.csproj` | Aspire host project — uses `Aspire.AppHost.Sdk` 13.2.2 |
| `AppHost/Program.cs` | Registers all services and their dependencies |
| `AppHost/Properties/launchSettings.json` | Dashboard URLs, OTLP endpoint, HTTPS profile |

### ServiceDefaults (shared library)

| File | Purpose |
|---|---|
| `aspire/ServiceDefaults/ServiceDefaults.csproj` | OpenTelemetry packages, service discovery |
| `aspire/ServiceDefaults/Extensions.cs` | `AddServiceDefaults()`, `MapDefaultEndpoints()` |

Every service calls `builder.AddServiceDefaults()` which wires up:
- OpenTelemetry (logging, metrics, tracing)
- Service discovery (resolves `services__*` env vars)
- Standard resilience handlers on HttpClient
- `/health` and `/alive` endpoints

## AppHost Program.cs

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis").WithLifetime(ContainerLifetime.Persistent);

var courseManagementService = builder.AddProject<Projects.lms_services_coursemanagement>(
    "coursemanagement", launchProfileName: "https")
    .WithReference(redis);

var userManagementService = builder.AddProject<Projects.lms_services_usermanagement>(
    "usermanagement", launchProfileName: "https")
    .WithReference(redis);

builder.AddProject<Projects.lms_web>("web", launchProfileName: "https")
    .WithReference(courseManagementService)
    .WithReference(userManagementService);

builder.Build().Run();
```

`WithReference()` injects:
- `services__coursemanagement__https__0=https://localhost:<port>`
- `services__coursemanagement__http__0=http://localhost:<port>`

## Running Aspire

```bash
cd AppHost
dotnet run --launch-profile https
```

Dashboard: `https://localhost:15889`

Requires:
- .NET 10 SDK
- Docker Desktop (for Redis container)
- Dev certificate trusted: `dotnet dev-certs https --trust`

## launchSettings.json (https profile)

```json
{
  "profiles": {
    "https": {
      "commandName": "Project",
      "applicationUrl": "https://localhost:15889;http://localhost:15888",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL": "https://localhost:16175",
        "DOTNET_RESOURCE_SERVICE_ENDPOINT_URL": "https://localhost:17013",
        "ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL": "https://localhost:16175"
      }
    }
  }
}
```

## Services in Aspire vs Docker Compose

| Concern | Aspire (dev) | Docker Compose |
|---|---|---|
| Service URLs | Auto-injected `services__*` env vars | Manually set in `docker-compose.yml` |
| Redis | Aspire starts container | `redis` service in compose |
| PostgreSQL | Commented out (use local or separate container) | `postgres` service in compose |
| Telemetry | Built-in dashboard at `https://localhost:15889` | Requires OTEL collector |
| AppHost | Runs on dev machine | Not used |

## OpenTelemetry

`Extensions.cs` checks for `OTEL_EXPORTER_OTLP_ENDPOINT` and activates OTLP export:

```csharp
private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
{
    bool useOtlpExporter = !string.IsNullOrWhiteSpace(
        builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
    if (useOtlpExporter)
        builder.Services.AddOpenTelemetry().UseOtlpExporter();
    return builder;
}
```

Aspire sets this automatically so logs/traces appear in the dashboard.  
In Docker Compose, set it manually to point to an OTEL collector if you need telemetry.

## NuGet Packages

### ServiceDefaults

| Package | Version |
|---|---|
| `Microsoft.Extensions.Http.Resilience` | 9.4.0 |
| `Microsoft.Extensions.ServiceDiscovery` | 9.4.0 |
| `OpenTelemetry.Exporter.OpenTelemetryProtocol` | 1.14.0 |
| `OpenTelemetry.Extensions.Hosting` | 1.14.0 |
| `OpenTelemetry.Instrumentation.AspNetCore` | 1.14.0 |
| `OpenTelemetry.Instrumentation.Http` | 1.14.0 |
| `OpenTelemetry.Instrumentation.Runtime` | 1.14.0 |

### AppHost

| Package | Version |
|---|---|
| `Aspire.Hosting` | 13.2.2 |
| `Aspire.Hosting.AppHost` | 13.2.2 |
| `Aspire.Hosting.PostgreSQL` | 13.2.2 |
| `Aspire.Hosting.Redis` | 13.2.2 |
