# eLearning Management System - Quick Start Guide

## 🚀 Start Development (Easiest)

### Using Aspire (Recommended for Development)

```bash
cd AppHost
dotnet run
```

Then open: **http://localhost:4317** (Aspire Dashboard)

The dashboard shows:
- All running services
- Real-time logs
- Performance metrics
- Service health status
- Direct links to APIs

### Using Docker Compose (Quickest)

```bash
docker-compose up -d
```

Wait for services to start, then access:
- **Web App**: http://localhost:5173
- **Course API**: http://localhost:8080/swagger
- **User API**: http://localhost:8081/swagger
- **Instructor API**: http://localhost:8082/swagger
- **PgAdmin**: http://localhost:5050 (admin / admin)
- **Redis**: localhost:6379

## 🛑 Stop Services

### Aspire
```bash
# Press Ctrl+C in the terminal
```

### Docker Compose
```bash
docker-compose down

# With volume cleanup
docker-compose down -v
```

## 📋 Common Commands

### View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f coursemanagement

# Last 100 lines
docker-compose logs --tail=100 coursemanagement
```

### Rebuild Images
```bash
docker-compose build --no-cache
```

### Check Service Status
```bash
docker-compose ps

# Detailed status
docker-compose ps --format "table {{.Name}}\t{{.Status}}"
```

### Access Service Shell
```bash
docker-compose exec coursemanagement bash
docker-compose exec postgres psql -U postgres
```

## 💾 Database Management

### Access PostgreSQL via CLI
```bash
# Connect to PostgreSQL
docker-compose exec postgres psql -U postgres

# List databases
\l

# Connect to a database
\c coursedb

# List tables
\dt

# Exit
\q
```

### Using PgAdmin (GUI)
1. Open: http://localhost:5050
2. Login: admin@example.com / admin
3. Create server connection:
   - Host: `postgres`
   - User: `postgres`
   - Password: `postgres`

### Create Databases (if needed)
```bash
docker-compose exec postgres psql -U postgres -c "CREATE DATABASE coursedb;"
docker-compose exec postgres psql -U postgres -c "CREATE DATABASE userdb;"
docker-compose exec postgres psql -U postgres -c "CREATE DATABASE instructordb;"
```

## 🔍 API Testing

### Using Swagger UI
- Course Management: http://localhost:8080
- User Management: http://localhost:8081
- Instructor Service: http://localhost:8082

Click "Try it out" to test endpoints directly.

### Using curl
```bash
# Health check
curl http://localhost:8080/health

# Get all courses (example)
curl http://localhost:8080/api/v1/courses

# Create a course (example)
curl -X POST http://localhost:8080/api/v1/courses \
  -H "Content-Type: application/json" \
  -d '{"name":"My Course","description":"Course description"}'
```

### Using Postman
1. Import the service Swagger definition
2. Example: http://localhost:8080/swagger/v1/swagger.json
3. Test endpoints from the imported collection

## 🐛 Debugging

### Check if port is in use
```bash
# On Windows
netstat -ano | findstr :8080

# On Linux/Mac
lsof -i :8080
```

### View container logs with timestamps
```bash
docker-compose logs --timestamps coursemanagement
```

### Verify network connectivity
```bash
# Ping between services
docker-compose exec coursemanagement ping usermanagement

# Check DNS resolution
docker-compose exec coursemanagement nslookup postgres
```

### Check environment variables
```bash
docker-compose exec coursemanagement env | grep ConnectionStrings
```

## 📁 Project Structure

```
AppHost/
├── AppHost.csproj          # Aspire host project
├── Program.cs              # Service orchestration
└── manifest.json           # Deployment manifest

aspire/ServiceDefaults/
├── ServiceDefaults.csproj  # Shared defaults
└── Extensions.cs           # Service extensions

src/
├── lms.services/
│   ├── lms.services.coursemanagement/
│   ├── lms.services.usermanagement/
│   └── lms.services.discount.gRPC/
├── lms.services.instructor/
├── lms.web/
└── lms.sln

docker-compose.yml         # Docker compose config
kubernetes-deployment.yaml # K8s manifests
DEPLOYMENT_GUIDE.md        # Detailed guide
```

## 🆘 Troubleshooting

### Services can't communicate
```bash
# Check Docker network
docker network ls
docker network inspect <network-name>

# Verify service names are correct
docker-compose ps
```

### Database connection fails
```bash
# Check PostgreSQL is running
docker-compose ps postgres

# Check logs
docker-compose logs postgres

# Verify connection string
docker-compose exec coursemanagement env | grep ConnectionStrings
```

### Port already in use
Edit `docker-compose.yml` and change port mapping:
```yaml
ports:
  - "8090:80"  # Change 8090 to any available port
```

### Out of disk space
```bash
# Clean up Docker
docker system prune -a --volumes

# Rebuild
docker-compose build --no-cache
```

## 🔄 Reload Changes

### Code Changes
- **Aspire**: Automatically restarts the service
- **Docker Compose**: 
  ```bash
  docker-compose restart <service-name>
  # or rebuild if dependencies changed
  docker-compose up -d --build <service-name>
  ```

### Configuration Changes
```bash
docker-compose up -d --build
```

## 📊 Performance Monitoring

### Aspire Dashboard Features
- **Services Tab**: See all running services
- **Logs Tab**: Aggregate logs from all services
- **Metrics Tab**: CPU, memory, request counts
- **Health Tab**: Service health status
- **Traces Tab**: Distributed tracing

### Check Service Health
```bash
curl http://localhost:8080/health
curl http://localhost:8081/health
curl http://localhost:8082/health
curl http://localhost:8083/health
curl http://localhost:5173/health
```

## 📚 Documentation

- **DEPLOYMENT_GUIDE.md** - Complete deployment instructions
- **ASPIRE_SETUP_SUMMARY.md** - Setup details
- **kubernetes-deployment.yaml** - Kubernetes configuration
- **docker-compose.yml** - Docker Compose configuration

## 🚀 Deployment

### Docker Registry Push
```bash
# Build image
docker build -f src/lms.services/lms.services.coursemanagement/Dockerfile.new \
  -t myregistry.azurecr.io/lms/coursemanagement:1.0.0 .

# Push to registry
docker push myregistry.azurecr.io/lms/coursemanagement:1.0.0
```

### Kubernetes Deployment
```bash
# Apply manifests
kubectl apply -f kubernetes-deployment.yaml

# Check status
kubectl get pods -n lms
kubectl get svc -n lms

# View logs
kubectl logs -n lms deployment/coursemanagement
```

## 💡 Pro Tips

1. **Keep one terminal for Aspire**: Easier to see logs and control everything
2. **Use Docker Desktop GUI**: Visual interface to manage containers
3. **Bookmark Swagger UIs**: Quick access to API docs
4. **Check Health Endpoints**: First step in debugging
5. **Use PgAdmin for DB queries**: No need to learn psql commands
6. **Enable Docker logs**: Right-click containers in Docker Desktop
7. **Set VSCode debug to Aspire**: Better debugging experience

## 🆘 Need Help?

1. Check **DEPLOYMENT_GUIDE.md** troubleshooting section
2. View service logs: `docker-compose logs -f <service>`
3. Check health endpoints: `curl http://localhost:xxxx/health`
4. Verify connectivity: `docker-compose exec <service> ping <other-service>`
5. Review configuration in `docker-compose.yml`

---

**Happy coding!** 🎉
