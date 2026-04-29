# Guía CI/CD - TaskCraft 🔄

Esta guía explica la configuración y uso del pipeline de CI/CD implementado con GitHub Actions.

## 📋 Tabla de Contenidos

- [Descripción General](#descripción-general)
- [Configuración del Pipeline](#configuración-del-pipeline)
- [Flujo de Trabajo](#flujo-de-trabajo)
- [Reportes de Cobertura](#reportes-de-cobertura)
- [Badges y Estados](#badges-y-estados)
- [Configuración Local](#configuración-local)
- [Troubleshooting](#troubleshooting)

## 🎯 Descripción General

El pipeline de CI/CD de TaskCraft automatiza:

- ✅ **Build automático** de toda la solución
- ✅ **Ejecución de tests** (199 tests unitarios)
- ✅ **Generación de reportes de cobertura**
- ✅ **Comentarios automáticos en PRs** con métricas
- ✅ **Artifacts de reportes** para revisión manual
- ✅ **Integración con Codecov** para tracking histórico

## ⚙️ Configuración del Pipeline

### Archivo Principal: `.github/workflows/ci.yml`

```yaml
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]
```

El pipeline se activa en:
- **Pushes** a `main` o `develop`
- **Pull Requests** hacia `main` o `develop`

### Variables de Entorno

```yaml
env:
  DOTNET_VERSION: '10.0.x'
  SOLUTION_PATH: './TaskCraft.slnx'
```

## 🔄 Flujo de Trabajo

### 1. Checkout del Código
```yaml
- name: Checkout code
  uses: actions/checkout@v4
  with:
    fetch-depth: 0
```
- Descarga el código del repositorio
- `fetch-depth: 0` necesario para análisis de cobertura completo

### 2. Setup de .NET
```yaml
- name: Setup .NET
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: ${{ env.DOTNET_VERSION }}
```
- Instala .NET 10 SDK
- Usa versión configurada en variables de entorno

### 3. Restauración de Dependencias
```yaml
- name: Restore dependencies
  run: dotnet restore ${{ env.SOLUTION_PATH }}
```
- Descarga todos los paquetes NuGet
- Cachea dependencias para futuras ejecuciones

### 4. Build de la Solución
```yaml
- name: Build solution
  run: dotnet build ${{ env.SOLUTION_PATH }} --configuration Release --no-restore
```
- Compila en modo Release
- Usa `--no-restore` para aprovechar cache

### 5. Ejecución de Tests con Coverage
```yaml
- name: Run tests with coverage
  run: dotnet test ${{ env.SOLUTION_PATH }} 
    --configuration Release 
    --no-build 
    --verbosity normal 
    --collect:"XPlat Code Coverage" 
    --results-directory ./coverage 
    --settings coverlet.runsettings
```

Parámetros clave:
- `--collect:"XPlat Code Coverage"`: Activa Coverlet
- `--results-directory ./coverage`: Directorio de salida
- `--settings coverlet.runsettings`: Configuración personalizada

### 6. Generación de Reportes
```yaml
- name: Install ReportGenerator
  run: dotnet tool install --global dotnet-reportgenerator-globaltool

- name: Generate coverage report
  run: reportgenerator 
    -reports:./coverage/**/coverage.cobertura.xml 
    -targetdir:./coverage-report 
    -reporttypes:"Html;MarkdownSummaryGithub"
```

Genera:
- **HTML Report**: Reporte visual completo
- **Markdown Summary**: Para comentarios en PRs

### 7. Upload de Artifacts
```yaml
- name: Upload coverage report
  uses: actions/upload-artifact@v4
  with:
    name: coverage-report
    path: ./coverage-report
    retention-days: 30
```
- Guarda reportes por 30 días
- Descargables desde la interfaz de GitHub Actions

### 8. Comentarios en PRs
```yaml
- name: Add coverage comment to PR
  if: github.event_name == 'pull_request'
  run: |
    if [ -f ./coverage-report/SummaryGithub.md ]; then
      cat ./coverage-report/SummaryGithub.md >> $GITHUB_STEP_SUMMARY
    fi
```
- Solo se ejecuta en PRs
- Agrega métricas al summary del job

## 📊 Reportes de Cobertura

### Archivo de Configuración: `coverlet.runsettings`

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat code coverage">
        <Configuration>
          <Format>cobertura,opencover</Format>
          <Exclude>[*.Tests]*,[*]*.Migrations.*</Exclude>
          <ExcludeByFile>**/Program.cs</ExcludeByFile>
          <IncludeDirectory>../</IncludeDirectory>
          <SingleHit>false</SingleHit>
          <UseSourceLink>true</UseSourceLink>
          <IncludeTestAssembly>false</IncludeTestAssembly>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

### Exclusiones de Coverage

- `[*.Tests]*` - Excluye proyectos de tests
- `[*]*.Migrations.*` - Excluye migraciones de EF Core
- `**/Program.cs` - Excluye archivo de configuración

### Formatos Generados

1. **Cobertura**: Para ReportGenerator
2. **OpenCover**: Para herramientas de análisis

## 🏷️ Badges y Estados

### Configurar Badges en README

```markdown
[![CI/CD Pipeline](https://github.com/YOUR_USERNAME/TaskCraft/actions/workflows/ci.yml/badge.svg)](https://github.com/YOUR_USERNAME/TaskCraft/actions/workflows/ci.yml)
[![Code Coverage](https://codecov.io/gh/YOUR_USERNAME/TaskCraft/branch/main/graph/badge.svg)](https://codecov.io/gh/YOUR_USERNAME/TaskCraft)
```

**Reemplaza** `YOUR_USERNAME` con tu usuario de GitHub.

### Estados del Pipeline

- ✅ **Passing**: Todos los checks pasaron
- ❌ **Failing**: Al menos un check falló
- 🟡 **Pending**: En ejecución
- ⚪ **No status**: Sin ejecuciones recientes

## 🖥️ Configuración Local

### Ejecutar Tests con Coverage Localmente

```bash
# 1. Ejecutar tests con coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage --settings coverlet.runsettings

# 2. Instalar ReportGenerator (solo primera vez)
dotnet tool install --global dotnet-reportgenerator-globaltool

# 3. Generar reporte HTML
reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage-report -reporttypes:Html

# 4. Abrir reporte
start ./coverage-report/index.html  # Windows
open ./coverage-report/index.html   # macOS
xdg-open ./coverage-report/index.html  # Linux
```

### Verificar Build antes de Push

```bash
# Build completo
dotnet build --configuration Release

# Ejecutar todos los tests
dotnet test --configuration Release --no-build

# Limpiar y rebuild
dotnet clean
dotnet build --configuration Release
dotnet test --configuration Release --no-build
```

## 🔧 Troubleshooting

### Tests Fallan en CI pero Pasan Localmente

**Posibles causas:**
1. Dependencias de archivo o rutas absolutas
2. Diferencias de timezone
3. Dependencias no restauradas correctamente

**Solución:**
```bash
# Limpiar todo
dotnet clean
rm -rf */bin */obj

# Restaurar y rebuild
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release --no-build
```

### Coverage Report No se Genera

**Verificar:**
1. Que `coverlet.collector` esté instalado en el proyecto de tests
2. Que el archivo `coverlet.runsettings` exista en la raíz
3. Que los tests realmente se ejecuten

**Debug:**
```bash
# Ejecutar con verbosity detallada
dotnet test --verbosity detailed --collect:"XPlat Code Coverage" --results-directory ./coverage --settings coverlet.runsettings

# Verificar archivos generados
ls -la ./coverage/**/coverage.cobertura.xml
```

### Artifacts No Disponibles

**Verificar:**
1. Que el workflow complete exitosamente
2. Que la retention no haya expirado (30 días por defecto)
3. Permisos del repositorio

### Badges No se Actualizan

**Soluciones:**
1. Forzar refresh con `Ctrl+F5` en el navegador
2. Verificar que la URL del badge sea correcta
3. Esperar 5-10 minutos (cache de GitHub)

## 📈 Mejoras Futuras

### Posibles Extensiones

- [ ] **Deploy Automático** a Azure/AWS en merge a main
- [ ] **Release Automation** con semantic versioning
- [ ] **Análisis de Seguridad** con herramientas SAST
- [ ] **Performance Benchmarks** automáticos
- [ ] **Docker Build** y push a registry
- [ ] **Notificaciones** a Slack/Discord
- [ ] **Scheduled Tests** nocturnos
- [ ] **Mutation Testing** con Stryker.NET

### Optimizaciones

- [ ] **Cache de NuGet** para builds más rápidos
- [ ] **Matrix Strategy** para múltiples versiones de .NET
- [ ] **Parallel Jobs** para tests y linting
- [ ] **Conditional Workflows** por tipo de cambio

## 🔗 Referencias

- [GitHub Actions Documentation](https://docs.github.com/actions)
- [Coverlet Documentation](https://github.com/coverlet-coverage/coverlet)
- [ReportGenerator](https://github.com/danielpalme/ReportGenerator)
- [.NET CLI Reference](https://docs.microsoft.com/dotnet/core/tools/)
- [Codecov Integration](https://docs.codecov.com/docs)

## 💡 Tips y Best Practices

### 1. **Ejecutar Tests Localmente Siempre**
```bash
# Antes de commit
dotnet test --configuration Release
```

### 2. **Mantener Tests Rápidos**
- Tests unitarios < 100ms cada uno
- Evitar dependencias externas en tests
- Usar mocks para servicios externos

### 3. **Monitorear Coverage**
- Objetivo: > 80% coverage
- Priorizar código crítico de negocio
- No forzar 100% en código trivial

### 4. **Revisar Reportes en PRs**
- Verificar que coverage no baje
- Revisar archivos sin cobertura
- Agregar tests para nuevas features

### 5. **Mantener Pipeline Verde**
- Fixes inmediatos si pipeline falla
- No hacer merge con tests fallidos
- Investigar flaky tests

---

**Configuración completada por**: GitHub Copilot
**Fecha**: 2025
**Estado**: ✅ Activo y funcionando
