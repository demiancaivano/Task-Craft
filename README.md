# TaskCraft API 🚀

[![CI/CD Pipeline](https://github.com/YOUR_USERNAME/TaskCraft/actions/workflows/ci.yml/badge.svg)](https://github.com/YOUR_USERNAME/TaskCraft/actions/workflows/ci.yml)
[![Code Coverage](https://codecov.io/gh/YOUR_USERNAME/TaskCraft/branch/main/graph/badge.svg)](https://codecov.io/gh/YOUR_USERNAME/TaskCraft)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

**TaskCraft** es una API RESTful moderna para gestión de proyectos y tareas, construida con .NET 10 y siguiendo principios de Clean Architecture.

## 🎯 Características Principales

- ✅ **Autenticación JWT** - Sistema seguro de autenticación y autorización
- ✅ **Clean Architecture** - Separación clara de responsabilidades
- ✅ **FluentValidation** - Validación robusta de datos con 13 validadores
- ✅ **Tests Unitarios** - 199 tests con 100% de cobertura en validadores
- ✅ **CI/CD** - Pipeline automatizado con GitHub Actions
- ✅ **Code Coverage** - Reportes de cobertura en cada PR
- ✅ **Swagger/OpenAPI** - Documentación interactiva de la API
- ✅ **CORS** - Configurado para integración frontend

## 🏗️ Arquitectura

```
TaskCraft/
├── TaskCraft.Core/           # Entidades, Enums, Interfaces base
├── TaskCraft.Application/    # DTOs, Servicios, Validadores
├── TaskCraft.Infrastructure/ # Repositorios, DbContext, Configuración
├── TaskCraft.API/            # Controllers, Middleware, Program.cs
└── TaskCraft.Tests/          # Tests unitarios (xUnit)
```

## 🚀 Inicio Rápido

### Prerrequisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB, Express, o completo)
- [Visual Studio 2026](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)

### Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/YOUR_USERNAME/TaskCraft.git
   cd TaskCraft
   ```

2. **Configurar la base de datos**

   Edita `TaskCraft.API/appsettings.json` con tu connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskCraftDb;Trusted_Connection=true;"
     }
   }
   ```

3. **Aplicar migraciones**
   ```bash
   dotnet ef database update --project TaskCraft.Infrastructure --startup-project TaskCraft.API
   ```

4. **Ejecutar la aplicación**
   ```bash
   dotnet run --project TaskCraft.API
   ```

5. **Acceder a Swagger**

   Abre tu navegador en: `https://localhost:7000/swagger`

## 🧪 Tests

### Ejecutar todos los tests
```bash
dotnet test
```

### Ejecutar tests con cobertura
```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
```

### Generar reporte HTML de cobertura
```bash
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage-report -reporttypes:Html
```

## 📊 Cobertura de Código

El proyecto mantiene alta cobertura de código con especial énfasis en:

- ✅ **199 tests unitarios** para validadores
- ✅ **100% cobertura** en capa de validación
- ✅ Tests para casos edge y escenarios de error
- ✅ Reportes automáticos en cada PR

Ver [TaskCraft.Tests/README.md](TaskCraft.Tests/README.md) para más detalles.

## 🔒 Autenticación

La API usa JWT Bearer tokens. Para autenticarte:

1. **Registrar usuario**
   ```http
   POST /api/auth/register
   Content-Type: application/json

   {
     "username": "usuario",
     "email": "usuario@ejemplo.com",
     "password": "Password123!",
     "firstName": "Nombre",
     "lastName": "Apellido"
   }
   ```

2. **Login**
   ```http
   POST /api/auth/login
   Content-Type: application/json

   {
     "username": "usuario",
     "password": "Password123!"
   }
   ```

3. **Usar el token**
   ```http
   GET /api/projects
   Authorization: Bearer {tu-token-aqui}
   ```

## 📚 Documentación

- [Guía de Validación](TaskCraft.API/Docs/VALIDATION_GUIDE.md) - Detalles sobre validadores y reglas
- [Tests Unitarios](TaskCraft.Tests/README.md) - Estructura y ejecución de tests
- [Validadores](TaskCraft.Application/Validators/README.md) - Catálogo de validadores

## 🔄 CI/CD Pipeline

El proyecto usa GitHub Actions para CI/CD:

- ✅ **Build automático** en cada push y PR
- ✅ **Tests automáticos** con reporte de resultados
- ✅ **Cobertura de código** con reportes visuales
- ✅ **Comentarios en PRs** con métricas de cobertura
- ✅ **Artifacts** de reportes de cobertura

Ver [.github/workflows/ci.yml](.github/workflows/ci.yml) para la configuración completa.

## 📦 Tecnologías

- **Framework**: .NET 10
- **ORM**: Entity Framework Core
- **Autenticación**: JWT Bearer
- **Validación**: FluentValidation
- **Base de datos**: SQL Server
- **Tests**: xUnit + FluentAssertions
- **Coverage**: Coverlet
- **API Docs**: Swagger/OpenAPI
- **CI/CD**: GitHub Actions

## 🤝 Contribuir

Las contribuciones son bienvenidas! Por favor:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

### Estándares de Código

- Todos los PRs deben pasar los tests
- Mantener cobertura de código > 80%
- Seguir convenciones de C# y .NET
- Incluir tests para nuevas funcionalidades

## 📝 Endpoints Principales

| Endpoint | Método | Descripción |
|----------|--------|-------------|
| `/api/auth/register` | POST | Registrar nuevo usuario |
| `/api/auth/login` | POST | Iniciar sesión |
| `/api/auth/refresh` | POST | Renovar token |
| `/api/users` | GET | Listar usuarios |
| `/api/projects` | GET/POST | Gestionar proyectos |
| `/api/projects/{id}` | GET/PUT/DELETE | Operaciones en proyecto específico |
| `/api/tasks` | GET/POST | Gestionar tareas |
| `/api/tasks/{id}` | GET/PUT/DELETE | Operaciones en tarea específica |
| `/api/comments` | GET/POST | Gestionar comentarios |
| `/api/tags` | GET/POST | Gestionar etiquetas |

Ver la documentación completa en Swagger UI.

## 📋 To-Do List - Features Opcionales

Las siguientes características están planeadas para futuras versiones:

- 🎨 **10. Email Service** - Servicio de notificaciones por correo electrónico
- 🎨 **11. File Upload** - Sistema de carga y gestión de archivos adjuntos
- 🎨 **12. SignalR for Real-time** - Comunicación en tiempo real para actualizaciones instantáneas

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo [LICENSE](LICENSE) para más detalles.

## 👥 Autores

- **Tu Nombre** - *Trabajo Inicial* - [TuGitHub](https://github.com/YOUR_USERNAME)

## 🙏 Agradecimientos

- Comunidad de .NET
- Contribuidores de FluentValidation
- Equipo de xUnit
- GitHub Actions

---

⭐ Si este proyecto te resulta útil, considera darle una estrella!
