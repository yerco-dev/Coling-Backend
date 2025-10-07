# Configuración del Proyecto Coling-Backend

## Pasos para ejecutar el proyecto

### 1. Verificar SQL Server LocalDB
```bash
# Verificar que SQL Server LocalDB esté instalado
sqllocaldb info

# Si no está instalado, descargar desde:
# https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb

# Crear instancia si no existe
sqllocaldb create mssqllocaldb

# Iniciar instancia
sqllocaldb start mssqllocaldb
```

### 2. Aplicar Migraciones de Base de Datos
```bash
# Desde la raíz del proyecto
cd src/Coling.Infrastructure

# Aplicar migraciones
dotnet ef database update --startup-project ../Coling.API
```

### 3. Configurar local.settings.json
El archivo `src/Coling.API/local.settings.json` ya debe estar configurado con:
- ConnectionString a SQL Server LocalDB
- Configuración JWT

### 4. Ejecutar el proyecto
```bash
cd src/Coling.API

# Opción 1: Usar Azure Functions Core Tools
func start

# Opción 2: Usar dotnet
dotnet run
```

### 5. Probar el endpoint
- Abrir `tests/register-member.http` en VS Code
- Usar la extensión REST Client para ejecutar las peticiones
- El endpoint estará disponible en: `http://localhost:7071/api/auth/register-member`

## Solución de Problemas

### Error: "The ConnectionString property has not been initialized"
- Verificar que `local.settings.json` tenga la sección `ConnectionStrings`
- Reiniciar la aplicación después de modificar `local.settings.json`

### Error: "Cannot open database"
- Ejecutar `dotnet ef database update` desde la carpeta Infrastructure
- Verificar que SQL Server LocalDB esté corriendo

### Error de Roles
- Los roles del sistema se crean automáticamente al iniciar la aplicación
- Verificar los logs para confirmar la creación de roles

## Estructura de Configuración

### appsettings.json (producción)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "Jwt": {
    "Key": "...",
    "Issuer": "...",
    "Audience": "..."
  }
}
```

### local.settings.json (desarrollo)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ColingDB;..."
  },
  "Jwt": {
    "Key": "...",
    "Issuer": "...",
    "Audience": "..."
  }
}
```
