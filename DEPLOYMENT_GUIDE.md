# eLearning Management System - Aspire Deployment Guide

## Overview

This guide explains how to deploy the eLearning Management System using .NET Aspire with Docker containers.

## Architecture

The system consists of the following microservices:

1. **Course Management Service** - Manages courses, modules, and enrollments
2. **User Management Service** - Handles user authentication and profile management
3. **Instructor Service** - Manages instructor-specific features and dashboards
4. **Discount gRPC Service** - Provides discount calculation via gRPC
5. **Web Application** - ASP.NET Core web frontend
6. **PostgreSQL Database** - Data persistence layer
7. **Redis Cache** - In-memory caching

## Prerequisites

- Docker and Docker Compose installed
- .NET 10 SDK (for development)
- Git

## Project Structure

```
├── AppHost/                                    # Aspire AppHost project
│   ├── AppHost.csproj
│   ├── Program.cs
│   └── manifest.json
├── aspire/
│   └── ServiceDefaults/                        # Shared service defaults
│       ├── ServiceDefaults.csproj
│       └── Extensions.cs
├── src/
│   ├── lms.services/
│   │   ├── lms.services.coursemanagement/     # Course microservice
│   │   ├── lms.services.usermanagement/       # User microservice
│   │   └── lms.services.discount.gRPC/        # Discount gRPC service
│   ├── lms.services.instructor/               # Instructor microservice
│   ├── lms.web/                               # Web application
│   ├── lms.buildingblocks/                    # Common utilities
│   ├── lms.shared.common/                     # Shared models
│   └── lms.shared.data/                       # Database contexts
├── docker-compose.yml                         # Docker Compose configuration
└── .dockerignore                              # Docker build exclusions
```

## Quick Start with Docker Compose

### 1. Build and Run All Services

```bash
# Navigate to the project root
cd eLearning-management

# Build and start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down

# Remove volumes (clean database)
docker-compose down -v
```

### 2. Access Services

After running `docker-compose up`, the services are available at:

- **Web Application**: http://localhost:5173
- **Course Management API**: http://localhost:8080
  - Swagger UI: http://localhost:8080/swagger
- **User Management API**: http://localhost:8081
  - Swagger UI: http://localhost:8081/swagger
- **Instructor Service**: http://localhost:8082
  - Swagger UI: http://localhost:8082/swagger
- **Discount gRPC Service**: http://localhost:8083
- **PgAdmin**: http://localhost:5050
  - Username: admin@example.com
  - Password: admin
- **Redis**: localhost:6379

### 3. Database Access via PgAdmin

1. Go to http://localhost:5050
2. Login with credentials above
3. Create a server connection:
   - Hostname: `postgres`
   - Username: `postgres`
   - Password: `postgres`
4. Create databases: `coursedb`, `userdb`, `instructordb`

## Development with Aspire (Running from Source)

### 1. Prerequisites

- Visual Studio 2022 or VS Code with C# extension
- .NET 10 SDK

### 2. Run Aspire AppHost

```bash
# Navigate to AppHost directory
cd AppHost

# Run the AppHost
dotnet run

# This will:
# 1. Start all services in debug mode
# 2. Launch the Aspire Dashboard at http://localhost:4317
# 3. Provide real-time monitoring of all services
```

### 3. Aspire Dashboard Features

- Real-time service monitoring
- Logs aggregation
- Metrics visualization
- Service endpoint access
- Health checks
- Performance diagnostics

## Production Deployment

### Using the Manifest File

The `AppHost/manifest.json` file contains the complete configuration for production deployment:

```bash
# Generate manifest for container orchestration
dotnet publish AppHost --no-build -o ./publish

# The manifest can be deployed to:
# - Kubernetes (via Aspire to K8s converter)
# - Azure Container Instances
# - Docker Swarm
# - Other orchestration platforms
```

### Docker Compose Production Configuration

For production, modify `docker-compose.yml`:

1. **Change environment variables**:
   ```yaml
   ASPNETCORE_ENVIRONMENT: Production
   ```

2. **Configure external services** (replace with production URLs):
   - AWS S3 endpoints
   - AWS SQS endpoints
   - External databases
   - CDN configurations

3. **Add resource limits**:
   ```yaml
   services:
     coursemanagement:
       deploy:
         resources:
           limits:
             cpus: '1'
             memory: 512M
   ```

4. **Configure persistent volumes** for databases:
   ```yaml
   volumes:
     postgres_data:
       driver: local
       driver_opts:
         type: nfs
   ```

## Kubernetes Deployment

### 1. Convert Aspire Manifest to Kubernetes

```bash
# Using Aspire CLI (if available)
dotnet publish AppHost --no-build -o ./kube -p PublishProfile=k8s

# Or manually create Kubernetes manifests from the Aspire resources
```

### 2. Deploy to Kubernetes

```bash
# Create namespace
kubectl create namespace lms

# Deploy services
kubectl apply -f kubernetes/

# Check deployment status
kubectl get pods -n lms
kubectl get svc -n lms

# View logs
kubectl logs -n lms deployment/coursemanagement
```

### 3. Sample Kubernetes Service Configuration

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: coursemanagement
  namespace: lms
spec:
  replicas: 3
  selector:
    matchLabels:
      app: coursemanagement
  template:
    metadata:
      labels:
        app: coursemanagement
    spec:
      containers:
      - name: coursemanagement
        image: your-registry/lms-coursemanagement:latest
        ports:
        - containerPort: 80
        env:
        - name: ConnectionStrings__CourseDatabase
          valueFrom:
            secretKeyRef:
              name: db-secrets
              key: course-connection-string
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
```

## Environment Configuration

### Development Environment

Set these in `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "CourseDatabase": "Server=localhost;Port=5432;Database=coursedb;User Id=postgres;Password=postgres;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

### Production Environment

Use environment variables or Azure Key Vault:

```bash
# Set connection string
export ConnectionStrings__CourseDatabase="Server=prod-db.postgres.database.azure.com;Database=coursedb;User Id=admin;Password=xxxxx;"

# Set Aspire environment
export ASPNETCORE_ENVIRONMENT="Production"
```

## Monitoring and Logging

### Aspire Dashboard

The AppHost provides built-in monitoring:

1. **Metrics**: CPU, memory, request latency
2. **Logs**: Centralized logging from all services
3. **Traces**: Distributed tracing
4. **Health**: Service health status

### External Monitoring (Serilog)

Services use Serilog for structured logging:

```json
{
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/lms-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

## Scaling

### Horizontal Scaling with Docker Compose

```bash
# Scale course management to 3 instances
docker-compose up -d --scale coursemanagement=3

# Use a load balancer (Nginx) to distribute traffic
```

### Load Balancing Example (Nginx)

```nginx
upstream coursemanagement {
  server coursemanagement:80;
  server coursemanagement-1:80;
  server coursemanagement-2:80;
}

server {
  listen 8080;
  location / {
    proxy_pass http://coursemanagement;
  }
}
```

## Health Checks

All services expose a `/health` endpoint:

```bash
# Check service health
curl http://localhost:8080/health

# Response:
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy"
  }
}
```

## Troubleshooting

### Common Issues

1. **Port Already in Use**
   ```bash
   # Change port in docker-compose.yml
   ports:
     - "8090:80"  # Map to different host port
   ```

2. **Database Connection Errors**
   ```bash
   # Check if PostgreSQL is running
   docker-compose logs postgres

   # Verify connection string
   docker-compose exec coursemanagement env | grep ConnectionStrings
   ```

3. **Services Can't Connect to Each Other**
   ```bash
   # Verify network
   docker network ls
   docker network inspect <network-name>

   # Test connectivity
   docker-compose exec coursemanagement ping usermanagement
   ```

4. **Volume Permission Issues**
   ```bash
   # Fix permissions
   sudo chown -R 999:999 postgres_data/
   ```

## Cleanup

```bash
# Remove all containers, networks, and volumes
docker-compose down -v

# Remove unused Docker resources
docker system prune -a
```

## Next Steps

1. Configure AWS services (S3, SQS) for production
2. Set up SSL/TLS certificates
3. Configure CI/CD pipeline
4. Set up monitoring and alerting
5. Implement backup strategies
6. Configure auto-scaling policies

## Additional Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Redis Documentation](https://redis.io/documentation)
