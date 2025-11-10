# BlueWhale - Docker Registry Management System

A complete, enterprise-ready Docker Registry management platform featuring a powerful .NET backend API and modern Nuxt web interface.

## 🎯 Overview

BlueWhale provides a comprehensive web-based management panel for Docker Registry with authentication, access control, and activity logging. It connects to the official Docker Registry API to manage repositories, images, tags, and user permissions.

### Components

| Component | Technology | Port | Purpose |
|-----------|-----------|------|---------|
| **Backend API** | .NET 9.0 | 5260 | Registry management, authentication, data persistence |
| **Frontend UI** | Nuxt 4 + Vue 3 | 3000 | Web interface for registry management |
| **Docker Registry** | Official Registry v2 | 5000 | Docker image storage and distribution |

## 📋 Features

### Backend (BlueWhale.Registry)
- ✅ REST API with Swagger documentation
- ✅ JWT authentication & authorization
- ✅ User management with role-based access
- ✅ Repository management
- ✅ Tag management
- ✅ Activity logging & audit trail
- ✅ Settings management
- ✅ SQLite database persistence

### Frontend (BlueWhale.UI)
- ✅ Modern dark theme UI
- ✅ Dashboard with statistics
- ✅ Repository browser
- ✅ Access control management
- ✅ Activity logs viewer
- ✅ System settings panel
- ✅ Responsive design
- ✅ Real-time updates

## 🏗️ Project Structure

```
BlueWhale/
├── BlueWhale.Registry/
│   ├── BlueWhale.Registry.Api/           # ASP.NET Core API
│   ├── BlueWhale.Registry.Application/   # Business logic
│   ├── BlueWhale.Registry.Domain/        # Domain models
│   ├── BlueWhale.Registry.Infrastructure/# Data access
│   ├── docker-compose.yml
│   ├── Dockerfile
│   └── README.md
├── BlueWhale.UI/
│   ├── app/
│   │   ├── layouts/
│   │   ├── pages/
│   │   └── app.vue
│   ├── docker-compose.yml
│   ├── Dockerfile
│   ├── nuxt.config.ts
│   └── README.md
├── AGENTS.md                             # Development guidelines
├── template.html                         # UI template reference
└── README.md                             # This file
```

## 🚀 Quick Start

### Prerequisites

- Docker & Docker Compose
- .NET 9.0 SDK (for local development)
- Node.js 18+ (for local frontend development)

### Option 1: Docker Compose (Recommended)

```bash
# Start all services
cd BlueWhale.Registry
docker-compose up --build

# Or from BlueWhale.UI
cd BlueWhale.UI
docker-compose up --build
```

Services will be available at:
- 🌐 Frontend: http://localhost:3000
- 🔧 Backend API: http://localhost:5260
- 📦 Docker Registry: http://localhost:5000
- 📚 Swagger Docs: http://localhost:5260/swagger

### Option 2: Local Development

#### Backend

```bash
cd BlueWhale.Registry/BlueWhale.Registry.Api
dotnet run
```

API available at `http://localhost:5260`

#### Frontend

```bash
cd BlueWhale.UI
npm install
npm run dev
```

UI available at `http://localhost:3000`

#### Docker Registry

```bash
docker run -d -p 5000:5000 registry:2
```

## 📚 Documentation

- [Backend API Documentation](./BlueWhale.Registry/README.md)
- [Frontend UI Documentation](./BlueWhale.UI/README.md)
- [Development Guidelines](./AGENTS.md)

## 🔐 Security

- JWT-based authentication
- Role-based access control (RBAC)
- Encrypted password storage
- Activity audit logging
- Environment-based secrets management
- HTTPS support

## 🗄️ Database

Uses SQLite for data persistence with EF Core ORM. Database automatically created on first run.

### Entities
- **User**: User accounts with roles
- **ActivityLog**: Audit trail
- **AccessControl**: Repository permissions
- **RegistrySetting**: Configuration

## 🔗 API Integration

### Authentication

```bash
POST /v1/api/Auth/Login
Content-Type: application/json

{
  "username": "admin",
  "password": "password"
}
```

Returns JWT token for subsequent requests.

### Repositories

```bash
GET /v1/api/Repositories
Authorization: Bearer {token}
```

### Tags

```bash
GET /v1/api/Tags/{repository}
Authorization: Bearer {token}
```

See Swagger documentation at `/swagger` for full API reference.

## 🐳 Docker Deployment

### Build Images

```bash
# Backend
docker build -f BlueWhale.Registry/BlueWhale.Registry.Api/Dockerfile -t bluewhale-registry-api .

# Frontend
docker build -f BlueWhale.UI/Dockerfile -t bluewhale-ui .
```

### Environment Variables

```bash
# Backend
ASPNETCORE_ENVIRONMENT=Production
REGISTRY_URL=http://registry:5000
JWT_SECRET=your-production-secret

# Frontend
NUXT_PUBLIC_API_BASE=http://api.example.com/v1/api
```

## ⚙️ Configuration

### Backend Settings

Edit `BlueWhale.Registry/BlueWhale.Registry.Api/appsettings.json`:

```json
{
  "Registry": {
    "RegistryUrl": "http://localhost:5000",
    "TimeoutSeconds": 30
  },
  "Jwt": {
    "Secret": "your-secret",
    "AccessTokenMinutes": 60
  }
}
```

### Frontend Settings

Edit `BlueWhale.UI/nuxt.config.ts`:

```typescript
runtimeConfig: {
  public: {
    apiBase: 'http://localhost:5260/v1/api'
  }
}
```

## 🧪 Testing

### Backend Tests

```bash
cd BlueWhale.Registry
dotnet test
```

### Frontend Tests

```bash
cd BlueWhale.UI
npm run test
```

## 🌐 Proxy Support

BlueWhale supports multiple proxy configurations:

1. **Hosted Mode**: Direct connection to Docker Registry
2. **Proxy Mode**: Pull-through cache for upstream registries
3. **Group Mode**: Multiple upstream registries
4. **Private Mode**: Private registry with authentication

Configure in Settings page or `appsettings.json`.

## 📊 Performance

- Optimized SQLite queries with indexing
- Redis caching support (optional)
- Nuxt SSR for fast page loads
- API rate limiting ready
- Connection pooling

## 🤝 Contributing

Please follow the guidelines in [AGENTS.md](./AGENTS.md):

1. Code style and conventions
2. Git workflow
3. Pull request process
4. Testing requirements

## 📝 License

MIT

## 🐛 Troubleshooting

### Backend won't start

```bash
# Clear cache and rebuild
cd BlueWhale.Registry
dotnet clean
dotnet build
dotnet run
```

### Frontend won't connect to API

- Check `NUXT_PUBLIC_API_BASE` environment variable
- Ensure backend is running on port 5260
- Check CORS settings in backend

### Docker Registry connection fails

- Verify Docker Registry is running: `curl http://localhost:5000/v2/`
- Update `REGISTRY_URL` environment variable
- Check network connectivity between containers

## 📞 Support

For issues and questions:
1. Check existing GitHub issues
2. Review [AGENTS.md](./AGENTS.md) guidelines
3. Create a new issue with detailed description

## 🗺️ Roadmap

- [ ] Kubernetes integration
- [ ] Advanced analytics
- [ ] Webhook support
- [ ] CI/CD pipeline integration
- [ ] Multi-registry management
- [ ] Advanced backup & recovery

---

**Built with ❤️ for the Docker community**
