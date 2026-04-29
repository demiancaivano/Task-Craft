# Branch Protection Rules Configuration

Este documento describe las reglas de protección de ramas recomendadas para el repositorio TaskCraft.

## 🔒 Configuración Recomendada

### Para la rama `main`

Ve a: **Settings** → **Branches** → **Add rule** → Branch name pattern: `main`

#### ✅ Reglas a Activar

1. **Require a pull request before merging**
   - ✅ Require approvals: 1
   - ✅ Dismiss stale pull request approvals when new commits are pushed
   - ✅ Require review from Code Owners (si usas CODEOWNERS)

2. **Require status checks to pass before merging**
   - ✅ Require branches to be up to date before merging
   - **Status checks requeridos:**
     - `build-and-test` (del workflow ci.yml)

3. **Require conversation resolution before merging**
   - ✅ All conversations must be resolved

4. **Require signed commits** (opcional pero recomendado)
   - ✅ Require signed commits

5. **Require linear history** (opcional)
   - ✅ Require linear history (para mantener un historial limpio)

6. **Do not allow bypassing the above settings**
   - ✅ Do not allow bypassing the above settings (incluye administradores)

### Para la rama `develop`

Mismo patrón pero con reglas más flexibles:

1. **Require a pull request before merging**
   - ✅ Require approvals: 1
   - ❌ Dismiss stale pull request approvals (más flexible para desarrollo)

2. **Require status checks to pass before merging**
   - ✅ Require branches to be up to date before merging
   - **Status checks requeridos:**
     - `build-and-test`

3. **Require conversation resolution before merging**
   - ✅ All conversations must be resolved

## 📋 Checklist para PRs

Antes de crear un Pull Request, verifica:

- [ ] ✅ Todos los tests pasan localmente
- [ ] ✅ Coverage no ha disminuido
- [ ] ✅ Código sigue convenciones del proyecto
- [ ] ✅ Commits tienen mensajes descriptivos
- [ ] ✅ Documentación actualizada (si aplica)
- [ ] ✅ Sin conflictos con rama base
- [ ] ✅ Cambios revisados por ti mismo primero

## 🎯 Flujo de Trabajo Recomendado

```bash
# 1. Crear rama desde develop
git checkout develop
git pull origin develop
git checkout -b feature/mi-nueva-funcionalidad

# 2. Hacer cambios y commits
git add .
git commit -m "feat: agregar nueva funcionalidad"

# 3. Ejecutar tests localmente
dotnet test

# 4. Push a GitHub
git push origin feature/mi-nueva-funcionalidad

# 5. Crear PR en GitHub
# GitHub → Pull Requests → New Pull Request

# 6. Esperar a que pasen los checks automáticos
# 7. Solicitar revisión
# 8. Hacer merge cuando esté aprobado
```

## 🚫 Qué NO Hacer

❌ **NO hacer push directo a `main` o `develop`**
❌ **NO hacer merge sin que pasen los tests**
❌ **NO ignorar warnings del linter**
❌ **NO hacer commits con código comentado sin razón**
❌ **NO aumentar complejidad sin documentar**
❌ **NO hacer merge con conversaciones sin resolver**

## 📊 Métricas de Calidad

El pipeline verifica automáticamente:

- ✅ **Build exitoso**: Código compila sin errores
- ✅ **Tests pasando**: 199/199 tests en verde
- ✅ **Coverage estable**: No debe bajar significativamente
- ✅ **Sin vulnerabilidades**: Dependencias seguras

## 🔧 Configuración de GitHub Secrets

Para funcionalidades avanzadas (opcional):

### Codecov Token

1. Ve a [codecov.io](https://codecov.io) y crea cuenta
2. Conecta tu repositorio
3. Copia el token
4. En GitHub: **Settings** → **Secrets and variables** → **Actions** → **New repository secret**
5. Nombre: `CODECOV_TOKEN`
6. Valor: Tu token de Codecov

## 📚 Referencias

- [GitHub Branch Protection Rules](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/defining-the-mergeability-of-pull-requests/about-protected-branches)
- [Requiring status checks](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/defining-the-mergeability-of-pull-requests/about-protected-branches#require-status-checks-before-merging)
- [Code review best practices](https://github.com/features/code-review)

---

⚙️ **Importante**: Estas reglas ayudan a mantener la calidad del código y prevenir errores en producción.
