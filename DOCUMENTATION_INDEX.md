# eLearning Management System - Documentation Index

## 📚 Overview

Your eLearning Management System has been fully configured for Docker and Kubernetes deployment using .NET Aspire. This index guides you to the right documentation for your needs.

## 🚀 Quick Start (5 minutes)

**Start here if you want to run the system immediately:**

→ **[QUICK_START.md](QUICK_START.md)** - Quick reference for developers
- One-command startup
- Common Docker commands
- API testing
- Database access
- Debugging tips

## 🏗️ Architecture & Setup (30 minutes)

**Read this to understand how everything works:**

→ **[ASPIRE_SETUP_SUMMARY.md](ASPIRE_SETUP_SUMMARY.md)** - Technical setup details
- What was implemented
- Files created and modified
- Service descriptions
- Architecture overview
- Environment variables
- Next steps checklist

## 📖 Comprehensive Deployment Guide (1-2 hours)

**Detailed instructions for all deployment scenarios:**

→ **[DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)** - Complete deployment guide
- Architecture details
- Quick start with Docker Compose
- Development with Aspire
- Production deployment options
- Kubernetes configuration
- Environment configuration
- Monitoring and logging
- Scaling strategies
- Troubleshooting guide

## 📋 Changes Summary

→ **[CHANGES_SUMMARY.txt](CHANGES_SUMMARY.txt)** - Executive summary
- Files created
- Files modified
- Services configured
- Key features implemented
- Quick command reference

## 🐳 Docker Configuration

→ **[docker-compose.yml](docker-compose.yml)** - Docker Compose configuration
- All services defined
- Networking setup
- Volume management
- Environment variables
- Health checks
- Port mappings

## ☸️ Kubernetes Configuration

→ **[kubernetes-deployment.yaml](kubernetes-deployment.yaml)** - Complete K8s manifests
- Namespace definition
- Secrets management
- ConfigMaps
- StatefulSets
- Deployments
- Services
- Persistent volumes
- Ingress configuration
- Health probes
- Resource limits

## 🔧 Source Code Structure

### Aspire AppHost
```
AppHost/
├── AppHost.csproj          # Project file with Aspire references
├── Program.cs              # Service orchestration and configuration
└── manifest.json           # Production deployment manifest
```

### Service Defaults
```
aspire/ServiceDefaults/
├── ServiceDefaults.csproj  # Shared defaults library
└── Extensions.cs           # Service extension methods
```

### Microservices
```
src/
├── lms.services/
│   ├── lms.services.coursemanagement/  # Course service
│   ├── lms.services.usermanagement/    # User service
│   └── lms.services.discount.gRPC/     # Discount gRPC service
├── lms.services.instructor/            # Instructor service
├── lms.web/                            # Web frontend
├── lms.buildingblocks/                 # Shared utilities
├── lms.shared.common/                  # Shared models
└── lms.shared.data/                    # Data access layer
```

## 🎯 How to Use This Documentation

### I want to...

**Start the system immediately:**
1. See: [QUICK_START.md](QUICK_START.md)
2. Run: `docker-compose up -d`

**Understand the architecture:**
1. See: [ASPIRE_SETUP_SUMMARY.md](ASPIRE_SETUP_SUMMARY.md)
2. Review: Architecture section

**Deploy to production:**
1. See: [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)
2. Follow: Production Deployment section
3. Use: [kubernetes-deployment.yaml](kubernetes-deployment.yaml)

**Debug a service:**
1. See: [QUICK_START.md](QUICK_START.md)
2. Follow: Troubleshooting section
3. Check: [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) for detailed troubleshooting

**Monitor services:**
1. See: [QUICK_START.md](QUICK_START.md)
2. Section: Performance Monitoring
3. Access Aspire Dashboard: http://localhost:4317

**Access the database:**
1. See: [QUICK_START.md](QUICK_START.md)
2. Section: Database Management
3. Use PgAdmin: http://localhost:5050

**Set up auto-scaling:**
1. See: [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)
2. Section: Scaling

**Configure monitoring:**
1. See: [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)
2. Section: Monitoring and Logging

## 📊 Service Endpoints

### Development (Docker Compose)

| Service | Port | Endpoint | Type |
|---------|------|----------|------|
| Web App | 5173 | http://localhost:5173 | Web |
| Course API | 8080 | http://localhost:8080/swagger | REST |
| User API | 8081 | http://localhost:8081/swagger | REST |
| Instructor API | 8082 | http://localhost:8082/swagger | REST |
| Discount gRPC | 8083 | http://localhost:8083 | gRPC |
| PostgreSQL | 5432 | localhost:5432 | Database |
| PgAdmin | 5050 | http://localhost:5050 | Web |
| Redis | 6379 | localhost:6379 | Cache |

### Development (Aspire)

| Service | Status | Logs | Metrics |
|---------|--------|------|---------|
| All services | Aspire Dashboard: http://localhost:4317 | Real-time | CPU, Memory, Latency |

## 🔄 Common Tasks

### Start Services
```bash
# Using Docker Compose (recommended for quick setup)
docker-compose up -d

# Using Aspire (recommended for development)
cd AppHost && dotnet run
```

### View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f coursemanagement
```

### Test API
```bash
# Health check
curl http://localhost:8080/health

# Via Swagger UI
# Open: http://localhost:8080/swagger
```

### Access Database
```bash
# Via PgAdmin (easier)
# Open: http://localhost:5050

# Via command line
docker-compose exec postgres psql -U postgres
```

### Rebuild Services
```bash
docker-compose build --no-cache
docker-compose up -d
```

## 📚 Technology Stack

- **.NET Aspire** - Service orchestration and monitoring
- **.NET 10** - Runtime framework
- **PostgreSQL 16** - Primary database
- **Redis 7** - Caching layer
- **Docker** - Containerization
- **Kubernetes** - Orchestration platform
- **GitHub Actions** - CI/CD pipeline

## 🚦 Status Indicators

### Health Checks
All services expose a `/health` endpoint:
```bash
curl http://localhost:8080/health
```

Response:
```json
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy",
    "cache": "Healthy"
  }
}
```

### Aspire Dashboard
Access at: http://localhost:4317
- Service status
- Real-time logs
- Performance metrics
- Health indicators

## 🔐 Security Notes

### Development
- Default credentials used (see docker-compose.yml)
- For local development only

### Production
- Change all default passwords
- Use secrets management (Kubernetes Secrets, Azure Key Vault)
- Enable SSL/TLS
- Configure network policies
- Use private registries for images

## 📞 Getting Help

1. **Quick answers**: See [QUICK_START.md](QUICK_START.md)
2. **Detailed info**: See [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)
3. **Troubleshooting**: See DEPLOYMENT_GUIDE.md Troubleshooting section
4. **Setup questions**: See [ASPIRE_SETUP_SUMMARY.md](ASPIRE_SETUP_SUMMARY.md)

## 📈 What's Next?

1. ✅ **Run locally** with Docker Compose
2. ✅ **Test all endpoints** via Swagger UI
3. ✅ **Monitor with** Aspire Dashboard
4. ⬜ **Deploy to staging** using kubernetes-deployment.yaml
5. ⬜ **Configure CI/CD** with GitHub Actions workflow
6. ⬜ **Set up production** database and backups
7. ⬜ **Implement monitoring** (Prometheus, Grafana)
8. ⬜ **Configure auto-scaling** for Kubernetes

## 📝 Notes

- All services are stateless and can be scaled
- Database handles persistence
- Redis provides distributed caching
- Service discovery is automatic
- Health checks are configured for all services
- Logging is centralized via Serilog

---

**Start with [QUICK_START.md](QUICK_START.md) for immediate results!**

For comprehensive information, read [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md).
