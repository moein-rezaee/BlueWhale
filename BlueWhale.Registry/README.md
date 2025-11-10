# BlueWhale.Registry - Docker Registry Management API

A comprehensive .NET 9.0 Web API service for managing Docker Registry instances with full authentication, access control, and activity logging.

## Features

- 🔐 **Authentication & Authorization**: JWT-based authentication with role-based access control
- 📦 **Repository Management**: List, create, and manage Docker repositories
- 🏷️ **Tag Management**: Manage image tags across repositories
- 👥 **Access Control**: User management with granular permissions
- 📊 **Activity Logging**: Complete audit trail of all operations
- 🔧 **Settings Management**: Configure registry behavior and storage
- 🐳 **Docker Registry Integration**: Direct integration with official Docker Registry

## Project Structure

```
BlueWhale.Registry/
├── BlueWhale.Registry.Api/          # ASP.NET Core Web API
├── BlueWhale.Registry.Application/  # Business logic & CQRS handlers
├── BlueWhale.Registry.Domain/       # Domain entities & interfaces
└── BlueWhale.Registry.Infrastructure/ # Data access & external services
```

## Prerequisites

- .NET 9.0 SDK
- Docker & Docker Compose (for containerized deployment)
- SQLite (for database)

## Configuration

### Environment Variables

Create a `.env` file in `BlueWhale.Registry.Api/`:

```env
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5260
REGISTRY_URL=http://localhost:5000
JWT_SECRET=your-jwt-secret-key-change-this-in-production
```

### appsettings.json

```json
{
  "Registry": {
    "RegistryUrl": "http://localhost:5000",
    "TimeoutSeconds": 30
  },
  "Jwt": {
    "Secret": "your-secret-key",
    "Issuer": "bluewhale.registry",
    "Audience": "bluewhale.registry",
    "AccessTokenMinutes": 60,
    "RefreshTokenDays": 7
  }
}
```

## Running Locally

### Development Mode

```bash
cd BlueWhale.Registry/BlueWhale.Registry.Api
dotnet run --launch-profile https
```

The API will be available at `http://localhost:5260`

### With Docker Compose

```bash
cd BlueWhale.Registry
docker-compose up --build
```

## API Endpoints

### Health Check
- `GET /` - Service health status

### Authentication
- `POST /v1/api/Auth/Login` - User login
- `POST /v1/api/Auth/Register` - User registration
- `POST /v1/api/Auth/Refresh` - Refresh JWT token

### Repositories
- `GET /v1/api/Repositories` - List all repositories
- `GET /v1/api/Repositories/{name}` - Get repository details
- `POST /v1/api/Repositories` - Create new repository
- `DELETE /v1/api/Repositories/{name}` - Delete repository

### Tags
- `GET /v1/api/Tags/{repository}` - List tags in repository
- `GET /v1/api/Tags/{repository}/{tag}` - Get tag details
- `DELETE /v1/api/Tags/{repository}/{tag}` - Delete tag

### Users & Access Control
- `GET /v1/api/Users` - List users
- `GET /v1/api/Users/{id}` - Get user details
- `POST /v1/api/Users` - Create user
- `PUT /v1/api/Users/{id}` - Update user
- `DELETE /v1/api/Users/{id}` - Delete user

### Activity Logs
- `GET /v1/api/Logs` - Get activity logs
- `GET /v1/api/Logs?userId={userId}` - Get user activity

### Settings
- `GET /v1/api/Settings` - Get all settings
- `GET /v1/api/Settings/{key}` - Get setting by key
- `PUT /v1/api/Settings/{key}` - Update setting

## Database

The API uses SQLite for data persistence. Database file: `registry.db`

### Entities
- **User**: User accounts with roles (Admin, User, ReadOnly)
- **ActivityLog**: Audit trail of all operations
- **AccessControl**: Repository-level permissions
- **RegistrySetting**: Configuration settings

### Migrations

Database is auto-created on first run using EF Core.

## Logging

Logs are written to console using Serilog. Configure log levels in `appsettings.json`:

```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
  }
}
```

## Docker Deployment

### Build Image

```bash
docker build -f BlueWhale.Registry/BlueWhale.Registry.Api/Dockerfile -t bluewhale-registry-api .
```

### Run Container

```bash
docker run -p 5260:5260 \
  -e REGISTRY_URL=http://registry:5000 \
  -e JWT_SECRET=your-secret \
  bluewhale-registry-api
```

## Security

- Secrets are loaded from environment variables only
- Passwords are hashed using BCrypt
- JWT tokens are signed with HS256
- All API endpoints require authentication
- Input validation using FluentValidation

## Error Handling

API returns standardized error responses:

```json
{
  "type": "https://api.example.com/errors/bad-request",
  "title": "Bad Request",
  "status": 400,
  "detail": "Invalid input",
  "traceId": "0HN3GBTM4P7FC:00000001"
}
```

## Contributing

1. Follow the AGENTS.md guidelines
2. Use proper naming conventions (PascalCase for public members)
3. Write unit tests for new features
4. Run `dotnet build` before committing

## License

MIT

## Support

For issues and questions, please create an issue in the repository.
