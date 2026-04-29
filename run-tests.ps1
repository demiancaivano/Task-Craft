#!/usr/bin/env pwsh
# Script para ejecutar tests con coverage localmente

param(
    [switch]$OpenReport = $false,
    [switch]$CleanFirst = $false
)

Write-Host "🧪 TaskCraft - Test Runner con Coverage" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Limpiar directorios previos si se solicita
if ($CleanFirst) {
    Write-Host "🧹 Limpiando directorios de coverage previos..." -ForegroundColor Yellow
    if (Test-Path "./coverage") { Remove-Item -Recurse -Force "./coverage" }
    if (Test-Path "./coverage-report") { Remove-Item -Recurse -Force "./coverage-report" }
    Write-Host "✓ Limpieza completada" -ForegroundColor Green
    Write-Host ""
}

# Ejecutar tests con coverage
Write-Host "🔍 Ejecutando tests con cobertura de código..." -ForegroundColor Yellow
$testResult = dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage --settings coverlet.runsettings --verbosity normal

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Los tests fallaron" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "✓ Tests completados exitosamente" -ForegroundColor Green
Write-Host ""

# Verificar si reportgenerator está instalado
Write-Host "📊 Verificando ReportGenerator..." -ForegroundColor Yellow
$reportGenInstalled = dotnet tool list --global | Select-String "dotnet-reportgenerator-globaltool"

if (-not $reportGenInstalled) {
    Write-Host "⚙️ Instalando ReportGenerator..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-reportgenerator-globaltool
    Write-Host "✓ ReportGenerator instalado" -ForegroundColor Green
} else {
    Write-Host "✓ ReportGenerator ya está instalado" -ForegroundColor Green
}
Write-Host ""

# Generar reporte HTML
Write-Host "📈 Generando reporte de cobertura..." -ForegroundColor Yellow
reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage-report -reporttypes:"Html;MarkdownSummary;Badges"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error al generar el reporte" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "✓ Reporte generado en ./coverage-report" -ForegroundColor Green
Write-Host ""

# Mostrar resumen si existe
if (Test-Path "./coverage-report/Summary.md") {
    Write-Host "📋 Resumen de Cobertura:" -ForegroundColor Cyan
    Write-Host "========================" -ForegroundColor Cyan
    Get-Content "./coverage-report/Summary.md"
    Write-Host ""
}

# Abrir reporte en el navegador si se solicita
if ($OpenReport) {
    $reportPath = Resolve-Path "./coverage-report/index.html"
    Write-Host "🌐 Abriendo reporte en el navegador..." -ForegroundColor Yellow

    if ($IsWindows -or $PSVersionTable.PSVersion.Major -lt 6) {
        Start-Process $reportPath
    } elseif ($IsMacOS) {
        open $reportPath
    } elseif ($IsLinux) {
        xdg-open $reportPath
    }
}

Write-Host "✅ Proceso completado exitosamente!" -ForegroundColor Green
Write-Host ""
Write-Host "💡 Tip: Usa -OpenReport para abrir automáticamente el reporte en tu navegador" -ForegroundColor Gray
Write-Host "💡 Tip: Usa -CleanFirst para limpiar reportes previos antes de ejecutar" -ForegroundColor Gray
