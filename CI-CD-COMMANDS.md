# 🛠️ Comandos Útiles de CI/CD

Este archivo contiene comandos útiles para trabajar con el pipeline de CI/CD localmente.

## 📦 Instalación de Herramientas

### Instalar ReportGenerator (para reportes de coverage)
```bash
dotnet tool install --global dotnet-reportgenerator-globaltool
```

### Actualizar ReportGenerator
```bash
dotnet tool update --global dotnet-reportgenerator-globaltool
```

## 🧪 Ejecutar Tests

### Tests Básicos
```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar tests con verbose
dotnet test --verbosity detailed

# Ejecutar tests de un proyecto específico
dotnet test TaskCraft.Tests/TaskCraft.Tests.csproj
```

### Tests con Cobertura
```bash
# Con el script PowerShell (Recomendado)
.\run-tests.ps1
.\run-tests.ps1 -OpenReport
.\run-tests.ps1 -CleanFirst -OpenReport

# Manual
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage --settings coverlet.runsettings
```

### Generar Reporte de Coverage
```bash
# Generar reporte HTML
reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage-report -reporttypes:"Html;MarkdownSummary;Badges"

# Solo generar badges
reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage-report -reporttypes:"Badges"

# Generar múltiples formatos
reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage-report -reporttypes:"Html;JsonSummary;Badges;Cobertura"
```

## 🏗️ Build

### Build Básico
```bash
# Compilar solución
dotnet build

# Compilar en Release
dotnet build --configuration Release

# Compilar sin restaurar paquetes
dotnet build --no-restore
```

### Build con Validaciones
```bash
# Build tratando warnings como errores
dotnet build --configuration Release /warnaserror

# Build con análisis de código
dotnet build /p:EnforceCodeStyleInBuild=true
```

## 🎨 Formato de Código

### Verificar Formato
```bash
# Verificar sin hacer cambios
dotnet format --verify-no-changes

# Verificar con detalles
dotnet format --verify-no-changes --verbosity diagnostic
```

### Aplicar Formato
```bash
# Aplicar formato a toda la solución
dotnet format

# Aplicar solo a archivos cambiados
dotnet format --include $(git diff --name-only --diff-filter=d | grep '\.cs$' | tr '\n' ' ')
```

## 🧹 Limpieza

### Limpiar Artefactos de Build
```bash
# Limpiar build artifacts
dotnet clean

# Limpiar y restaurar
dotnet clean && dotnet restore
```

### Limpiar Coverage
```bash
# PowerShell
Remove-Item -Recurse -Force ./coverage, ./coverage-report -ErrorAction SilentlyContinue

# Bash
rm -rf ./coverage ./coverage-report
```

## 📊 Análisis y Reportes

### Ver Resumen de Coverage
```bash
# PowerShell
if (Test-Path "./coverage-report/Summary.md") { Get-Content "./coverage-report/Summary.md" }

# Bash
cat ./coverage-report/Summary.md 2>/dev/null || echo "No coverage report found"
```

### Análisis de Código
```bash
# Análisis estático
dotnet build /p:RunAnalyzersDuringBuild=true

# Análisis de seguridad (requiere herramientas adicionales)
dotnet list package --vulnerable
```

## 🔄 Restauración de Paquetes

```bash
# Restaurar paquetes
dotnet restore

# Restaurar con limpieza de cache
dotnet restore --force

# Restaurar y verificar integridad
dotnet restore --verify-no-hash-mismatch
```

## 🐛 Debugging de Tests

### Tests con Filtros
```bash
# Ejecutar tests de una clase específica
dotnet test --filter ClassName~TaskService

# Ejecutar un test específico
dotnet test --filter FullyQualifiedName~TaskCraft.Tests.TaskServiceTests.ShouldCreateTask

# Ejecutar tests por categoría (si usas traits)
dotnet test --filter Category=Unit
```

### Tests con Logger
```bash
# Generar archivo TRX
dotnet test --logger "trx;LogFileName=test-results.trx"

# Logger detallado en consola
dotnet test --logger "console;verbosity=detailed"
```

## 🚀 Simular CI Localmente

### Ejecutar todos los checks de CI
```bash
# PowerShell
Write-Host "🔍 Running CI checks locally..." -ForegroundColor Cyan

# 1. Restaurar
Write-Host "`n📦 Restoring packages..." -ForegroundColor Yellow
dotnet restore

# 2. Build
Write-Host "`n🏗️ Building..." -ForegroundColor Yellow
dotnet build --configuration Release --no-restore /warnaserror

# 3. Formato
Write-Host "`n🎨 Checking format..." -ForegroundColor Yellow
dotnet format --verify-no-changes

# 4. Tests con coverage
Write-Host "`n🧪 Running tests..." -ForegroundColor Yellow
.\run-tests.ps1

Write-Host "`n✅ All CI checks completed!" -ForegroundColor Green
```

### Script Bash Equivalente
```bash
#!/bin/bash
echo "🔍 Running CI checks locally..."

echo -e "\n📦 Restoring packages..."
dotnet restore

echo -e "\n🏗️ Building..."
dotnet build --configuration Release --no-restore /warnaserror

echo -e "\n🎨 Checking format..."
dotnet format --verify-no-changes

echo -e "\n🧪 Running tests..."
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage --settings coverlet.runsettings

echo -e "\n✅ All CI checks completed!"
```

## 📈 Métricas de Código

### Ver Paquetes Instalados
```bash
# Listar todos los paquetes
dotnet list package

# Paquetes desactualizados
dotnet list package --outdated

# Paquetes vulnerables
dotnet list package --vulnerable
```

### Información del Proyecto
```bash
# Ver información de la solución
dotnet sln list

# Ver referencias de un proyecto
dotnet list TaskCraft.API/TaskCraft.API.csproj reference
```

## 🔧 Configuración de GitHub Actions Localmente

### Act (ejecutar GitHub Actions localmente)
```bash
# Instalar act (Windows con Chocolatey)
choco install act-cli

# Instalar act (macOS con Homebrew)
brew install act

# Instalar act (Linux)
curl https://raw.githubusercontent.com/nektos/act/master/install.sh | sudo bash

# Ejecutar workflow
act

# Ejecutar workflow específico
act -W .github/workflows/ci.yml

# Ejecutar job específico
act -j build-and-test
```

## 💡 Tips y Trucos

### Ejecutar Tests en Paralelo
```bash
# Ejecutar con máximo paralelismo
dotnet test --parallel

# Limitar paralelismo
dotnet test --parallel --max-parallel 4
```

### Watch Mode (desarrollo continuo)
```bash
# Auto-ejecutar tests al cambiar código
dotnet watch test

# Auto-build al cambiar código
dotnet watch build
```

### Caché de Compilación
```bash
# Habilitar caché de compilación incremental
dotnet build /p:UseSharedCompilation=true
```

## 📝 Notas

- Todos los comandos asumen que estás en el directorio raíz de la solución
- Los comandos de PowerShell funcionan en Windows, macOS y Linux (PowerShell Core)
- Para comandos bash, usa Git Bash en Windows o terminal nativa en macOS/Linux

---

**Última Actualización**: 29/04/2026
