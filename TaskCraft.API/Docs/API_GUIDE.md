# 🚀 TaskCraft API - Developer Guide

## 📋 Table of Contents

- [Quick Start](#quick-start)
- [Architecture](#architecture)
- [API Documentation](#api-documentation)
- [Authentication](#authentication)
- [Error Handling](#error-handling)
- [Database](#database)
- [Testing](#testing)
- [Deployment](#deployment)

---

## 🎯 Quick Start

### Prerequisites

- .NET 10 SDK
- SQL Server or SQL Server Express
- Visual Studio 2026 or VS Code
- Postman (optional)

### Setup

1. **Clone Repository**
   ```bash
   git clone https://github.com/taskcraft/api.git
   cd taskcraft-api
   ```

2. **Configure Database**
   - Update `appsettings.json` with your SQL Server connection string
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=TaskCraftDB;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

3. **Apply Migrations**
   ```bash
   dotnet ef database update --project TaskCraft.Infrastructure
   ```

4. **Run Application**
   ```bash
   dotnet run --project TaskCraft.API
   ```

5. **Access Swagger**
   - Open browser: `https://localhost:5001/`
   - You should see Swagger UI with all endpoints

---

## 🏗️ Architecture

### Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│                        TaskCraft.API                         │
│                    (Presentation Layer)                      │
│  • Controllers                                               │
│  • Middleware (Error Handling)                               │
│  • Swagger Configuration                                     │
│  • JWT Authentication                                        │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│                   TaskCraft.Application                      │
│                     (Business Logic)                         │
│  • Services (UserService, ProjectService, etc.)              │
│  • DTOs (Data Transfer Objects)                              │
│  • Interfaces (IUserService, IProjectService, etc.)          │
│  • Mappings (Entity ↔ DTO)                                  │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│                      TaskCraft.Core                          │
│                     (Domain Layer)                           │
│  • Entities (User, Project, Task, etc.)                      │
│  • Interfaces (IUserRepository, IUnitOfWork, etc.)           │
│  • Custom Exceptions                                         │
│  • Business Rules                                            │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│                  TaskCraft.Infrastructure                    │
│                   (Data Access Layer)                        │
│  • DbContext (EF Core)                                       │
│  • Repositories (Implementation)                             │
│  • Migrations                                                │
│  • Unit of Work                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 📚 API Documentation

### Swagger/OpenAPI

**Accessing Documentation:**
- Development: `https://localhost:5001/`
- Swagger JSON: `https://localhost:5001/swagger/v1/swagger.json`

**Features:**
- ✅ Interactive API testing
- ✅ JWT authentication support
- ✅ Request/response examples
- ✅ Schema validation
- ✅ Try It Out functionality

**See:** [SWAGGER_GUIDE.md](./SWAGGER_GUIDE.md) for detailed usage

---

## 🌐 CORS Configuration

### Cross-Origin Resource Sharing (CORS)

TaskCraft API is configured to accept requests from specific frontend origins to ensure security while allowing your React/Vue/Angular applications to communicate with the API.

**Current Policy:** `AllowReactApp`

**Configuration:**

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173"
    ]
  }
}
```

**What's Allowed:**
- ✅ Origins: `http://localhost:3000` (React), `http://localhost:5173` (Vite)
- ✅ Methods: All HTTP methods (GET, POST, PUT, DELETE, PATCH, etc.)
- ✅ Headers: All headers
- ✅ Credentials: Cookies and authentication headers

### Adding New Origins

**Development Environment:**

1. Open `appsettings.Development.json` (or `appsettings.json`)
2. Add your origin to the array:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173",
      "http://localhost:4200",  // Angular
      "http://localhost:8080"   // Your custom port
    ]
  }
}
```

**Production Environment:**

Add your production domain:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://app.taskcraft.com",
      "https://www.taskcraft.com"
    ]
  }
}
```

### Security Best Practices

⚠️ **Important:**
- Never use `"*"` (allow all origins) in production
- Always use HTTPS in production (`https://`)
- Specify exact domains, avoid wildcards when possible
- Keep `AllowCredentials()` only if you need cookies/auth headers

**See:** [CORS_GUIDE.md](./CORS_GUIDE.md) for detailed configuration

---

## 🔐 Authentication

### JWT Bearer Authentication

**Flow:**

1. **Login/Register** → Get Access Token + Refresh Token
2. **Use Access Token** → Make API requests (valid for 1 hour)
3. **Refresh Token** → Get new Access Token (valid for 7 days)
4. **Revoke Token** → Logout

### Configuration

```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-min-32-characters",
    "Issuer": "TaskCraftAPI",
    "Audience": "TaskCraftClient",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

### Usage Example

```http
# 1. Login
POST /api/auth/login
Content-Type: application/json

{
  "usernameOrEmail": "john@example.com",
  "password": "Password123!"
}

# Response:
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "a1b2c3...",
  "expiresIn": 3600
}

# 2. Make Authenticated Request
GET /api/users
Authorization: Bearer eyJhbGc...

# 3. Refresh Token (when expired)
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "a1b2c3..."
}
```

---

## ⚠️ Error Handling

### Global Error Middleware

All errors are caught and returned in a standardized format:

```json
{
  "statusCode": 404,
  "errorCode": "NOT_FOUND",
  "message": "User with identifier '123...' was not found.",
  "timestamp": "2024-01-15T10:30:00.000Z",
  "path": "/api/users/123..."
}
```

### Custom Exceptions

| Exception | HTTP Code | Usage |
|-----------|-----------|-------|
| `NotFoundException` | 404 | Resource not found |
| `ValidationException` | 400 | Validation errors |
| `UnauthorizedException` | 401 | Authentication failed |
| `ForbiddenException` | 403 | Authorization failed |
| `ConflictException` | 409 | Resource already exists |
| `BadRequestException` | 400 | Invalid request |

### Service Layer Example

```csharp
public async Task<User> GetUserByIdAsync(Guid id)
{
    var user = await _unitOfWork.Users.GetByIdAsync(id);

    if (user == null)
        throw new NotFoundException(nameof(User), id);

    return user;
}
```

**See:** [ERROR_HANDLING.md](./ERROR_HANDLING.md) for complete guide

---

## 🗄️ Database

### Entity Framework Core

**Migrations:**

```bash
# Add Migration
dotnet ef migrations add MigrationName --project TaskCraft.Infrastructure

# Update Database
dotnet ef database update --project TaskCraft.Infrastructure

# Remove Last Migration
dotnet ef migrations remove --project TaskCraft.Infrastructure

# Generate SQL Script
dotnet ef migrations script --project TaskCraft.Infrastructure
```

### Entities

- **User** - User accounts and authentication
- **Project** - Project management
- **ProjectMember** - Project team members
- **Task** - Tasks within projects
- **TaskAssignment** - Task assignments to users
- **RefreshToken** - JWT refresh tokens

### Relationships

```
User ──┐
       ├──< ProjectMember >──< Project
       │
       └──< TaskAssignment >──< Task >──< Project
```

---

## 🧪 Testing

### Using Swagger UI

1. Navigate to `https://localhost:5001/`
2. Click "Authorize" button
3. Login to get JWT token
4. Test all endpoints interactively

### Using Postman

1. Import OpenAPI spec: `https://localhost:5001/swagger/v1/swagger.json`
2. Set environment variable: `{{baseUrl}}` = `https://localhost:5001`
3. Create authentication request
4. Save token to environment
5. Test endpoints

### Manual Testing Checklist

- [ ] User registration
- [ ] User login
- [ ] Token refresh
- [ ] Create project
- [ ] Add project members
- [ ] Create tasks
- [ ] Assign tasks
- [ ] Update entities
- [ ] Delete entities
- [ ] Error scenarios

---

## 🚀 Deployment

### Configuration

**Production appsettings:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Production connection string"
  },
  "JwtSettings": {
    "SecretKey": "production-secret-key",
    "RequireHttpsMetadata": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Environment Variables

```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection="Server=..."
JwtSettings__SecretKey="your-production-secret"
```

### Docker (Optional)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskCraft.API.dll"]
```

---

## 📦 Project Structure

```
TaskCraft/
│
├── TaskCraft.API/
│   ├── Controllers/          # API Controllers
│   ├── Middleware/           # Custom Middleware
│   ├── Models/               # API Models (ErrorResponse)
│   ├── Swagger/              # Swagger Configuration
│   ├── Docs/                 # Documentation
│   ├── Program.cs            # Application Entry Point
│   └── appsettings.json      # Configuration
│
├── TaskCraft.Application/
│   ├── Services/             # Business Logic
│   ├── Interfaces/           # Service Interfaces
│   ├── DTOs/                 # Data Transfer Objects
│   └── Mappings/             # Entity ↔ DTO Mappings
│
├── TaskCraft.Core/
│   ├── Entities/             # Domain Models
│   ├── Interfaces/           # Repository Interfaces
│   └── Exceptions/           # Custom Exceptions
│
└── TaskCraft.Infrastructure/
    ├── Data/                 # DbContext
    ├── Repositories/         # Repository Implementations
    ├── UnitOfWork/           # Unit of Work Pattern
    └── Migrations/           # EF Core Migrations
```

---

## 🔧 Development Tools

### Recommended VS Extensions

- **REST Client** - Test APIs directly in VS Code
- **C# Dev Kit** - Enhanced C# support
- **EF Core Power Tools** - Visualize database
- **Thunder Client** - API testing

### Useful Commands

```bash
# Build solution
dotnet build

# Run tests
dotnet test

# Watch mode (auto-reload)
dotnet watch run --project TaskCraft.API

# Clean solution
dotnet clean

# Restore packages
dotnet restore
```

---

## 📖 Additional Resources

- [Swagger Guide](./SWAGGER_GUIDE.md) - Complete Swagger documentation
- [Error Handling](./ERROR_HANDLING.md) - Error handling system
- [CORS Guide](./CORS_GUIDE.md) - Cross-Origin Resource Sharing configuration
- [Frontend Integration](./FRONTEND_INTEGRATION.md) - React, Vue, Angular examples
- [Migration Guide](./MIGRATION_GUIDE.md) - Migration instructions

---

## 👥 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request

---

## 📄 License

This project is licensed under the MIT License.

---

## 📞 Support

- **Email:** support@taskcraft.com
- **Issues:** https://github.com/taskcraft/api/issues
- **Docs:** https://docs.taskcraft.com

---

**Happy Coding! 🚀**
