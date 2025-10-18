# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Coling-Backend** is an ASP.NET Core 8.0 application built using Azure Functions v4 with a Clean Architecture pattern. It provides a member management system with authentication and authorization for a professional organization (Colegio de Ingenieros).

The application uses:
- **Azure Functions** for serverless HTTP endpoints
- **ASP.NET Core Identity** for user authentication
- **Entity Framework Core 8.0** with SQL Server
- **JWT tokens** for stateless authentication
- **Clean Architecture** (Domain, Application, Infrastructure, API layers)

## Build and Run Commands

### Build the Solution
```bash
dotnet build Coling-Backend.sln
```

### Run Database Migrations
```bash
# From the Infrastructure project directory
cd src/Coling.Infrastructure
dotnet ef database update --startup-project ../Coling.API

# Or from the root directory
dotnet ef database update --project src/Coling.Infrastructure --startup-project src/Coling.API
```

### Create New Migration
```bash
cd src/Coling.Infrastructure
dotnet ef migrations add <MigrationName> --startup-project ../Coling.API
```

### Drop and Recreate Database
```bash
# Drop database
dotnet ef database drop --project src/Coling.Infrastructure --startup-project src/Coling.API --force

# Recreate with migrations
dotnet ef database update --project src/Coling.Infrastructure --startup-project src/Coling.API
```

### Run the Application
```bash
# Using Azure Functions Core Tools (preferred)
cd src/Coling.API
func start

# Or using dotnet CLI
cd src/Coling.API
dotnet run
```

The API will be available at `http://localhost:7071` (Azure Functions default port).

### Testing
HTTP request files are located in the `tests/` directory:
- `login.http` - Authentication tests
- `register-member.http` - Member registration tests
- `members-management.http` - Member management operations
- `academic-management.http` - Academic education CRUD operations (Degree, Continuing Education, Professional Certification)
- `institution-management.http` - Institution and Institution Type CRUD operations

Use VS Code REST Client extension or similar tools to execute these requests.

## Architecture and Code Structure

### Clean Architecture Layers

The codebase follows Clean Architecture with clear separation of concerns:

#### 1. **Domain Layer** (`Coling.Domain`)
- **Core business entities**: `User`, `Person`, `Member`, `Role`
- **Business constants**: `MemberStatus`, `SystemRoles` with Spanish localized values
- **Repository interfaces**: Define data access contracts
- **No dependencies** on other layers - this is the innermost layer

Key entities relationship:
- `User` (Identity) → `Person` (1:1) → `Member` (optional 1:1)
- A Person can exist without being a Member
- A User must have an associated Person

#### 2. **Application Layer** (`Coling.Application`)
- **Use Cases**: Encapsulate business logic (e.g., `RegisterMemberUserUseCase`, `LoginUseCase`, `ApproveMemberUseCase`)
- **DTOs**: Request/Response objects for data transfer
- **Validators**: FluentValidation for input validation
- **Mappers**: Convert between entities and DTOs
- **Service interfaces**: Define application services (e.g., `ITokenService`)
- **Depends only on**: Domain layer

Use cases follow a consistent pattern:
- Input validation via DTOs
- Business logic execution
- Return structured responses with `ActionResponse<T>`

#### 3. **Infrastructure Layer** (`Coling.Infrastructure`)
- **Data access**: Entity Framework Core `AppDbContext`
- **Repository implementations**: Concrete implementations of domain repository interfaces
- **External services**: JWT token generation (`JwtService`), email services, etc.
- **Database configuration**: Migrations, entity configurations, unique indexes
- **Seeders**: `UsersSeeder` creates default roles and admin user on startup
- **Unit of Work pattern**: `DbContextUnitOfWork` for transaction management
- **User data loading**: `GetFullData` method includes User → Person → Member eager loading
- **Depends on**: Domain and Application layers

#### 4. **API Layer** (`Coling.API`)
- **Azure Functions endpoints**: HTTP-triggered functions organized by feature
- **Middleware**: Custom JWT authentication middleware (`JwtAuthenticationMiddleware`)
- **Dependency injection**: Configured in `Program.cs`
- **Authorization policies**: `AdminOnly`, `AdminOrModerator`, `MemberOnly`
- **Depends on**: All other layers

Endpoints are organized by domain:
- `EndPoints/UsersManagement/` - Authentication endpoints (login, register)
- `EndPoints/MembersManagement/` - Member management (approve, reject, list pending)
- `EndPoints/AcademicManagement/` - Member education CRUD (Degree Education, Continuing Education, Professional Certification)
- `EndPoints/InstitutionManagement/` - Institution and InstitutionType CRUD (create, read, update, soft delete)

### Key Design Patterns

1. **Repository Pattern**: Data access abstraction
2. **Unit of Work**: Transaction boundary management
3. **Use Case Pattern**: Each business operation is encapsulated
4. **DTO Pattern**: Separate external contracts from domain models
5. **Dependency Injection**: All dependencies registered in `Program.cs`

### Authentication & Authorization Flow

1. **JWT Authentication**: Custom middleware in `Middleware/JwtAuthenticationMiddleware.cs` validates tokens
2. **Role-Based Authorization**: Three roles with Spanish names:
   - "Administrador" (Admin)
   - "Moderador" (Moderator)
   - "Miembro" (Member)
3. **Authorization Policies**: Defined in `Program.cs:58-70`
4. **Member Status Workflow**:
   - New member registers → Status: "Pendiente de Verificación"
   - Admin/Moderator approves → Status: "Verificado", Role: "Miembro"
   - Admin/Moderator rejects → Status: "Suspendido", Account deactivated
5. **Login Restrictions**: Members with "Pendiente de Verificación" status cannot log in (returns 404 in `LoginUseCase.cs:57-65`)

### Database Configuration

- **Provider**: SQL Server (LocalDB for development)
- **Connection String**: Configured in `local.settings.json` (development) or `appsettings.json` (production)
- **Design-Time DbContext**: `AppDbContextFactory` in Infrastructure/Data enables EF Core commands to work
- **Identity**: Custom `User` and `Role` entities extending ASP.NET Core Identity with Guid primary keys
- **Unique Constraints**:
  - `Person.NationalId` (unique per person)
  - `Member.MembershipCode` (unique per member)
  - `Member.TitleNumber` (unique per member)

### Important Configuration Files

- `local.settings.json` - Local development settings (connection strings, JWT config)
- `appsettings.json` - Production/shared settings
- `host.json` - Azure Functions host configuration
- `Program.cs` - Application startup, DI registration, middleware pipeline

### Validation Rules

Password validation (configured in `Program.cs:43-47`):
- Minimum 8 characters, maximum 100
- Requires digit, lowercase, uppercase, and non-alphanumeric character
- Unique email required across users

Uniqueness validations (performed before registration):
- **UserName**: Must be unique across all users
- **NationalId**: Must be unique across all persons (validated via `PersonValidator`)
- **MembershipCode**: Must be unique across all members (validated via `MemberValidator`)
- **TitleNumber**: Must be unique across all members (validated via `MemberValidator`)

### Response Structure

All API responses follow a consistent format defined in `ActionResponse<T>`:
```csharp
{
  "wasSuccessful": bool,
  "message": string,
  "data": T | null,
  "errors": string[] | null
}
```

### Localization

- All business constants, error messages, and user-facing strings are in **Spanish**
- Role names and member statuses use Spanish values throughout the system

## Development Guidelines

### Adding a New Endpoint

1. Create DTOs in `Application/DTOs/`
2. Create validator for request DTO in `Application/Validators/`
3. Create use case in `Application/UseCases/[Feature]/`
4. Implement any needed repositories in `Infrastructure/Repositories/[Feature]/`
5. Create Azure Function endpoint in `API/EndPoints/[Feature]/`
6. Register use case in `Program.cs` services configuration
7. Add HTTP test file in `tests/` directory

### Adding a New Entity

1. Create entity in `Domain/Entities/[Feature]/`
2. Add DbSet to `AppDbContext.cs`
3. Configure relationships in `AppDbContext.OnModelCreating`
4. Create repository interface in `Domain/Interfaces/Repositories/[Feature]/`
5. Implement repository in `Infrastructure/Repositories/[Feature]/`
6. Register repository in `Program.cs`
7. Create and apply migration

### Database Seeding

Default data is seeded on application startup via `UsersSeeder` (called in `Program.cs:97-110`):
- Creates system roles if they don't exist
- Creates default admin user with credentials in configuration

## Common Issues

### SQL Server LocalDB
If database connection fails, verify LocalDB is running:
```bash
sqllocaldb info
sqllocaldb start mssqllocaldb
```

### Migration Issues
Always run migrations from the Infrastructure project with API as startup project:
```bash
dotnet ef database update --project src/Coling.Infrastructure --startup-project src/Coling.API
```

### JWT Token Issues
- Tokens expire after 24 hours (configured in JWT service)
- `JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear()` in `Program.cs:25` ensures standard claim names

### Role-Based Authorization
When adding new policies, use Spanish role names from `BusinessConstants.SystemRolesValues`:
```csharp
policy.RequireRole(BusinessConstants.SystemRolesValues[SystemRoles.Admin])
```

## API Modules and Endpoints

The system includes the following functional modules with complete CRUD operations:

### Academic Management Module

Allows members to manage their educational records including degrees, continuing education, and professional certifications.

#### Degree Education (Grados Académicos)
- `POST /api/academic/degree-education` - Register degree education (Licenciatura, Maestría, Doctorado)
- `GET /api/academic/degree-education` - List all degree educations for authenticated member
- `PUT /api/academic/degree-education/{id}` - Update degree education (only owner)
- `DELETE /api/academic/degree-education/{id}` - Soft delete degree education (only owner)

#### Continuing Education (Educación Continua)
- `POST /api/academic/continuing-education` - Register continuing education (Cursos, Talleres, Seminarios)
- `GET /api/academic/continuing-education` - List all continuing education for authenticated member
- `PUT /api/academic/continuing-education/{id}` - Update continuing education (only owner)
- `DELETE /api/academic/continuing-education/{id}` - Soft delete continuing education (only owner)

#### Professional Certification (Certificaciones Profesionales)
- `POST /api/academic/professional-certification` - Register professional certification
- `GET /api/academic/professional-certification` - List all certifications for authenticated member
- `PUT /api/academic/professional-certification/{id}` - Update certification (only owner)
- `DELETE /api/academic/professional-certification/{id}` - Soft delete certification (only owner)

**Key Features:**
- File upload support with Azure Blob Storage integration
- Ownership validation - members can only modify their own records
- Flexible date handling (PartialDate value object)
- Soft delete pattern (IsActive flag)
- Multipart/form-data for POST and PUT with optional file replacement

### Institution Management Module

Manages institutions and their types for use in academic records.

#### Institution Types (Tipos de Institución)
- `POST /api/institution-type` - Register institution type (Universidad, Empresa, etc.)
- `GET /api/institution-type` - List all institution types
- `GET /api/institution-type/{id}` - Get institution type by ID
- `PUT /api/institution-type/{id}` - Update institution type
- `DELETE /api/institution-type/{id}` - Soft delete institution type

#### Institutions (Instituciones)
- `POST /api/institution` - Register institution
- `GET /api/institution` - List all institutions with type names
- `GET /api/institution/{id}` - Get institution by ID with type name
- `PUT /api/institution/{id}` - Update institution
- `DELETE /api/institution/{id}` - Soft delete institution

**Key Features:**
- JSON-based requests (no file uploads)
- Case-insensitive duplicate validation
- Soft delete pattern
- Foreign key validation (InstitutionType must exist)
- Response includes related data (institution type name)

### Work Experience Module (Planned)

Future endpoints for managing work experience and work fields. Entities already exist in domain:
- `WorkFieldCategory` - Categories of work fields
- `WorkField` - Specific work areas/fields
- `WorkExperience` - Member work experience records
- `WorkExperienceField` - Many-to-many relationship between work experience and fields

Repositories and database configuration already implemented. Endpoints pending implementation.

## Important Implementation Notes

### File Upload Pattern
Academic Management endpoints support optional file uploads:
1. Use `multipart/form-data` content type
2. Files stored in Azure Blob Storage (container: `academic-documents`)
3. Naming pattern: `{memberId}_{guid}.{extension}`
4. On update with new file: old file is deleted from storage
5. Transaction rollback if file upload fails

### Soft Delete Pattern
All delete operations are soft deletes:
- Entity is marked as `IsActive = false`
- Entity remains in database for auditing
- Files in blob storage are preserved
- Queries typically filter by `IsActive = true`

### Ownership Validation
Academic Management endpoints validate ownership:
1. Extract `MemberId` from JWT token
2. Compare with record's `MemberId`
3. Return 403 Forbidden if mismatch
4. Only applies to Update and Delete operations

### PartialDate Value Object
Flexible date representation supporting:
- Year only: `{ year: 2020 }`
- Year + Month: `{ year: 2020, month: 6 }`
- Full date: `{ year: 2020, month: 6, day: 15 }`

Used for education start/end dates and certification dates.

### Entity Relationships
```
Member (1) ──< MemberEducation (junction) >── Education (1)
                                              ├── DegreeEducation
                                              ├── ContinuingEducation
                                              └── ProfessionalCertification

Institution (N) ──< Education
Institution (N) ──> InstitutionType (1)
```

## Documentation

- `docs/API-ENDPOINTS.md` - Complete API documentation with request/response examples
- `docs/INSTITUTION-ENDPOINTS-ADICIONALES.md` - Additional Institution Management endpoints documentation
- `tests/*.http` - HTTP request files for testing with REST Client extension
