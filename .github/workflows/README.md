# CI/CD Pipeline - TaskCraft

Este directorio contiene la configuración de GitHub Actions para el pipeline de CI/CD del proyecto TaskCraft.

## Workflows Configurados

### 1. CI/CD Pipeline (`ci.yml`)

**Triggers:**
- Push a las ramas `main` y `develop`
- Pull Requests a las ramas `main` y `develop`

**Acciones:**
- ✅ Checkout del código
- ✅ Configuración de .NET 10
- ✅ Restauración de dependencias
- ✅ Build de la solución en modo Release
- ✅ Ejecución de tests con cobertura de código
- ✅ Generación de reportes HTML de coverage
- ✅ Publicación de artefactos de coverage
- ✅ Comentario automático con resumen de coverage en PRs
- ✅ Publicación de resultados de tests

**Reportes Generados:**
- Reporte HTML de cobertura (disponible como artefacto)
- Resumen Markdown en el PR
- Resultados de tests unitarios

### 2. Code Quality (`code-quality.yml`)

**Triggers:**
- Pull Requests a las ramas `main` y `develop`

**Acciones:**
- ✅ Validación de formato de código con `dotnet format`
- ✅ Build con warnings tratados como errores
- ✅ Verificación de estándares de código

## Configuración Local

### Ejecutar Tests con Coverage

```bash
# Ejecutar tests con coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage --settings coverlet.runsettings

# Generar reporte HTML
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage-report -reporttypes:"Html;MarkdownSummary"

# Abrir reporte
start ./coverage-report/index.html  # Windows
open ./coverage-report/index.html   # macOS
xdg-open ./coverage-report/index.html # Linux
```

### Verificar Formato de Código

```bash
# Verificar formato sin hacer cambios
dotnet format --verify-no-changes

# Aplicar formato automáticamente
dotnet format
```

## Badges para README Principal

Puedes agregar estos badges al README principal del proyecto:

```markdown
![CI/CD Pipeline](https://github.com/USUARIO/REPO/actions/workflows/ci.yml/badge.svg)
![Code Quality](https://github.com/USUARIO/REPO/actions/workflows/code-quality.yml/badge.svg)
```

## Configuración de Coverage

El archivo `coverlet.runsettings` en la raíz del proyecto configura:
- Formatos de salida: Cobertura y OpenCover
- Exclusiones: Proyectos de tests y migraciones
- Archivos excluidos: Program.cs
- Inclusión de SourceLink para mapeo de código fuente

## Umbrales de Cobertura Recomendados

Para agregar umbrales mínimos de cobertura, actualiza el archivo `.csproj` del proyecto de tests:

```xml
<PropertyGroup>
  <Threshold>80</Threshold>
  <ThresholdType>line</ThresholdType>
  <ThresholdStat>total</ThresholdStat>
</PropertyGroup>
```

## Artefactos

Los reportes de cobertura se mantienen disponibles por 30 días en la sección de artefactos de cada workflow run.

## Troubleshooting

### El workflow falla en la restauración
- Verificar que todas las dependencias estén disponibles en NuGet
- Revisar la versión de .NET especificada en el workflow

### No se genera el reporte de coverage
- Asegurar que coverlet.collector esté instalado en el proyecto de tests
- Verificar que los tests se ejecuten correctamente

### El formato de código falla
- Ejecutar `dotnet format` localmente antes de hacer commit
- Revisar las configuraciones de EditorConfig si existen

## Próximos Pasos

- [ ] Configurar integración con SonarCloud/SonarQube
- [ ] Agregar análisis de seguridad con CodeQL
- [ ] Configurar deployment automático a staging
- [ ] Implementar semantic versioning automático
