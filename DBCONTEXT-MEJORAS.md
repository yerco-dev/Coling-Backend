# Mejoras Opcionales para AppDbContext

Este documento contiene sugerencias de mejoras opcionales para el `AppDbContext` que pueden implementarse en futuras iteraciones cuando se necesite optimizar el rendimiento y aplicar mejores prácticas avanzadas.

**Fecha de creación**: 2025-10-10
**Estado**: Pendiente de implementación

---

## 📊 Tabla de Contenidos

1. [Query Filters Globales (Soft Delete)](#1-query-filters-globales-soft-delete)
2. [Índices de Performance en Foreign Keys](#2-índices-de-performance-en-foreign-keys)
3. [Índices de Búsqueda Comunes](#3-índices-de-búsqueda-comunes)
4. [Índice Compuesto Único en MemberEducation](#4-índice-compuesto-único-en-membereducation)
5. [Nombres de Tablas Explícitos](#5-nombres-de-tablas-explícitos-ya-aplicado-parcialmente)
6. [Configuración de Precisión para Decimales](#6-configuración-de-precisión-para-decimales)
7. [Configuraciones Avanzadas de TPT](#7-configuraciones-avanzadas-de-tpt)

---

## 1. Query Filters Globales (Soft Delete)

### Descripción
Los Query Filters permiten filtrar automáticamente registros con `IsActive = false` en todas las consultas. Esto evita tener que agregar `.Where(x => x.IsActive)` manualmente en cada query.

### Ventajas
- ✅ Reduce código repetitivo
- ✅ Previene errores de olvidar filtrar por `IsActive`
- ✅ Mejora consistencia en el manejo de soft delete

### Desventajas
- ❌ Puede causar confusión si no se documenta bien
- ❌ Requiere `.IgnoreQueryFilters()` cuando se necesiten incluir inactivos

### Implementación

```csharp
// En OnModelCreating, agregar:

// Entidades base
modelBuilder.Entity<Person>().HasQueryFilter(p => p.IsActive);
modelBuilder.Entity<Member>().HasQueryFilter(m => m.IsActive);

// Instituciones
modelBuilder.Entity<Institution>().HasQueryFilter(i => i.IsActive);
modelBuilder.Entity<InstitutionType>().HasQueryFilter(it => it.IsActive);

// Educación
modelBuilder.Entity<Education>().HasQueryFilter(e => e.IsActive);
modelBuilder.Entity<MemberEducation>().HasQueryFilter(me => me.IsActive);

// Trabajo
modelBuilder.Entity<WorkFieldCategory>().HasQueryFilter(wfc => wfc.IsActive);
modelBuilder.Entity<WorkField>().HasQueryFilter(wf => wf.IsActive);
modelBuilder.Entity<WorkExperience>().HasQueryFilter(we => we.IsActive);
modelBuilder.Entity<WorkExperienceField>().HasQueryFilter(wef => wef.IsActive);

// Usuarios
modelBuilder.Entity<Role>().HasQueryFilter(r => r.IsActive);
modelBuilder.Entity<User>().HasQueryFilter(u => u.IsActive);
```

### Uso

```csharp
// Query normal - solo registros activos
var activeMembers = await _context.Members.ToListAsync();

// Incluir inactivos
var allMembers = await _context.Members
    .IgnoreQueryFilters()
    .ToListAsync();
```

### Cuándo Implementar
- ✅ Cuando el equipo esté familiarizado con el patrón
- ✅ Cuando se documente en CLAUDE.md
- ✅ En sprint de refactorización de queries

---

## 2. Índices de Performance en Foreign Keys

### Descripción
Crear índices en columnas de Foreign Keys para mejorar el rendimiento de queries con JOINs y búsquedas por relación.

### Ventajas
- ✅ Mejora significativa en performance de JOINs
- ✅ Acelera queries que filtran por FK (ej: `GetMembersByPersonId`)
- ✅ Bajo costo de implementación

### Desventajas
- ❌ Incrementa ligeramente el tamaño de la BD
- ❌ Puede ralentizar INSERTs/UPDATEs (mínimo)

### Implementación

```csharp
// Relaciones User/Person/Member
modelBuilder.Entity<Member>()
    .HasIndex(m => m.PersonId);

modelBuilder.Entity<User>()
    .HasIndex(u => u.PersonId);

// Relaciones MemberEducation
modelBuilder.Entity<MemberEducation>()
    .HasIndex(me => me.MemberId);

modelBuilder.Entity<MemberEducation>()
    .HasIndex(me => me.EducationId);

// Relaciones WorkExperience
modelBuilder.Entity<WorkExperience>()
    .HasIndex(we => we.MemberId);

modelBuilder.Entity<WorkExperience>()
    .HasIndex(we => we.InstitutionId);

// Relación Education → Institution
modelBuilder.Entity<Education>()
    .HasIndex(e => e.InstitutionId);

// Relaciones WorkField
modelBuilder.Entity<WorkField>()
    .HasIndex(wf => wf.WorkFieldCategoryId);

// Relaciones Institution
modelBuilder.Entity<Institution>()
    .HasIndex(i => i.InstitutionTypeId);
```

### Cuándo Implementar
- ✅ Cuando se detecten queries lentas en producción
- ✅ Antes de lanzar a producción con datos reales
- ✅ Cuando se realice análisis de performance

---

## 3. Índices de Búsqueda Comunes

### Descripción
Crear índices en columnas que se usan frecuentemente en búsquedas y filtros.

### Ventajas
- ✅ Mejora significativa en búsquedas de texto
- ✅ Acelera filtros y ordenamientos
- ✅ Mejor experiencia de usuario en autocomplete/search

### Desventajas
- ❌ Incrementa tamaño de BD
- ❌ Requiere análisis de patrones de búsqueda

### Implementación

```csharp
// Búsquedas de personas (autocomplete, búsqueda de miembros)
modelBuilder.Entity<Person>()
    .HasIndex(p => p.FirstNames);

modelBuilder.Entity<Person>()
    .HasIndex(p => p.PaternalLastName);

modelBuilder.Entity<Person>()
    .HasIndex(p => p.MaternalLastName);

// Índice compuesto para búsqueda de nombre completo (más eficiente)
modelBuilder.Entity<Person>()
    .HasIndex(p => new { p.FirstNames, p.PaternalLastName, p.MaternalLastName });

// Búsqueda de instituciones
modelBuilder.Entity<Institution>()
    .HasIndex(i => i.Name);

// Filtro por estado de miembro (dashboards, reportes)
modelBuilder.Entity<Member>()
    .HasIndex(m => m.Status);

// Búsqueda de educaciones por nombre
modelBuilder.Entity<Education>()
    .HasIndex(e => e.Name);

// Búsqueda de campos de trabajo
modelBuilder.Entity<WorkField>()
    .HasIndex(wf => wf.Name);

modelBuilder.Entity<WorkFieldCategory>()
    .HasIndex(wfc => wfc.Name);
```

### Análisis Previo Requerido
Antes de implementar, responder:
1. ¿Qué campos se buscan más frecuentemente?
2. ¿Hay búsquedas de texto parcial (LIKE '%text%')?
3. ¿Se ordenan resultados por estos campos?
4. ¿Hay autocomplete implementado?

### Cuándo Implementar
- ✅ Después de analizar logs de queries
- ✅ Cuando se implementen funcionalidades de búsqueda
- ✅ En base a feedback de usuarios sobre lentitud

---

## 4. Índice Compuesto Único en MemberEducation

### Descripción
Crear un índice único compuesto en `MemberEducation` para prevenir duplicados (mismo miembro, misma educación).

### Pregunta de Negocio
❓ **¿Un miembro puede registrar la misma educación múltiples veces?**

Escenarios posibles:
- **NO**: Un miembro hace "Maestría en Ingeniería Civil" en UMSA una sola vez → Crear índice único
- **SÍ**: Un miembro puede repetir un curso → NO crear índice único

### Implementación (si la respuesta es NO)

```csharp
modelBuilder.Entity<MemberEducation>()
    .HasIndex(me => new { me.MemberId, me.EducationId })
    .IsUnique();
```

### Ventajas (si se implementa)
- ✅ Previene duplicados a nivel de BD
- ✅ Validación automática sin código adicional

### Desventajas
- ❌ Si el negocio cambia, requiere migración

### Cuándo Implementar
- ✅ Después de confirmar regla de negocio con stakeholders
- ✅ Documentar decisión en DOMAIN.md

---

## 5. Nombres de Tablas Explícitos (Ya Aplicado Parcialmente)

### Descripción
Definir explícitamente nombres de tablas en lugar de confiar en convenciones de EF Core.

### Estado Actual
✅ Ya implementado para TPT:
- `Educations`, `DegreeEducations`, `ContinuingEducations`, `ProfessionalCertifications`
- `Parties`, `Persons`, `Institutions`

### Pendiente de Implementar (si se desea 100% explícito)

```csharp
// Entidades que faltan
modelBuilder.Entity<Member>().ToTable("Members");
modelBuilder.Entity<InstitutionType>().ToTable("InstitutionTypes");
modelBuilder.Entity<MemberEducation>().ToTable("MemberEducations");
modelBuilder.Entity<WorkFieldCategory>().ToTable("WorkFieldCategories");
modelBuilder.Entity<WorkField>().ToTable("WorkFields");
modelBuilder.Entity<WorkExperience>().ToTable("WorkExperiences");
modelBuilder.Entity<WorkExperienceField>().ToTable("WorkExperienceFields");

// Tablas de Identity (si se desea renombrar)
modelBuilder.Entity<User>().ToTable("Users");
modelBuilder.Entity<Role>().ToTable("Roles");
// Nota: Identity genera más tablas (UserRoles, UserClaims, etc.)
```

### Ventajas
- ✅ Control total sobre nombres
- ✅ Evita sorpresas con cambios de convenciones de EF
- ✅ Facilita migraciones entre versiones de EF

### Desventajas
- ❌ Más código verboso
- ❌ Duplica información (el nombre ya está implícito)

### Cuándo Implementar
- ✅ Si se requiere naming estricto en BD
- ✅ En proyectos con DBAs estrictos
- ⚠️ Opcional para proyectos pequeños/medianos

---

## 6. Configuración de Precisión para Decimales

### Descripción
Configurar precisión de campos `decimal` en Fluent API (redundante con Data Annotations, pero más centralizado).

### Implementación

```csharp
// GPA en DegreeEducation (ya está en data annotation)
modelBuilder.Entity<DegreeEducation>()
    .Property(de => de.GPA)
    .HasPrecision(5, 2); // 999.99

// Si se agregan campos monetarios en el futuro
// modelBuilder.Entity<Member>()
//     .Property(m => m.MembershipFee)
//     .HasPrecision(10, 2); // Para montos de dinero
```

### Ventajas
- ✅ Centraliza configuración de BD
- ✅ Evita depender solo de data annotations

### Desventajas
- ❌ Duplica la configuración si ya está en data annotation
- ❌ Más código

### Recomendación
⚠️ **NO implementar** si ya se usa `[Column(TypeName = "decimal(5,2)")]` en la entidad. Solo agregar si se prefiere Fluent API sobre Data Annotations.

---

## 7. Configuraciones Avanzadas de TPT

### Descripción
Configuraciones adicionales para optimizar el comportamiento de Table Per Type.

### Configuraciones Opcionales

#### 7.1 Primary Keys Explícitas en Tablas Hijas

```csharp
// Asegurar que las tablas hijas usen el mismo Id que la tabla padre
modelBuilder.Entity<DegreeEducation>()
    .ToTable("DegreeEducations")
    .HasKey(de => de.Id); // Explícito

modelBuilder.Entity<ContinuingEducation>()
    .ToTable("ContinuingEducations")
    .HasKey(ce => ce.Id);

modelBuilder.Entity<ProfessionalCertification>()
    .ToTable("ProfessionalCertifications")
    .HasKey(pc => pc.Id);
```

#### 7.2 Índices en Tablas Base para Queries Polimórficas

```csharp
// Si se hacen muchas queries que devuelven todos los tipos de Education
modelBuilder.Entity<Education>()
    .HasIndex(e => e.IsActive); // Para queries polimórficas

modelBuilder.Entity<Education>()
    .HasIndex(e => new { e.InstitutionId, e.IsActive }); // Compuesto
```

### Cuándo Implementar
- ✅ Si se detectan problemas de performance con queries polimórficas
- ✅ En revisión de arquitectura de BD

---

## 📊 Priorización Recomendada

### Fase 1 - Esenciales para Producción
1. ✅ **Índices en Foreign Keys** (Sección 2) - Alto impacto, bajo costo
2. ✅ **Índice único en MemberEducation** (Sección 4) - Si la regla de negocio lo requiere

### Fase 2 - Optimizaciones de Performance
3. ⚠️ **Índices de búsqueda** (Sección 3) - Basado en análisis de queries reales
4. ⚠️ **Query Filters globales** (Sección 1) - Requiere capacitación del equipo

### Fase 3 - Refinamiento (Opcional)
5. 💡 **Nombres explícitos** (Sección 5) - Solo si se requiere control estricto
6. 💡 **Configuraciones TPT avanzadas** (Sección 7) - Solo si hay problemas de performance

---

## 📝 Checklist Pre-Implementación

Antes de implementar cualquier mejora, verificar:

- [ ] ¿Se ha analizado el impacto en performance actual?
- [ ] ¿Se han identificado los patrones de consulta más comunes?
- [ ] ¿Se ha documentado la decisión en DOMAIN.md o CLAUDE.md?
- [ ] ¿Se ha creado una migración de prueba en ambiente de desarrollo?
- [ ] ¿Se han actualizado los tests unitarios/integración?
- [ ] ¿El equipo conoce los cambios (ej: Query Filters)?

---

## 🔗 Referencias

- [EF Core Performance](https://learn.microsoft.com/en-us/ef/core/performance/)
- [EF Core Indexes](https://learn.microsoft.com/en-us/ef/core/modeling/indexes)
- [EF Core Query Filters](https://learn.microsoft.com/en-us/ef/core/querying/filters)
- [Table Per Type (TPT)](https://learn.microsoft.com/en-us/ef/core/modeling/inheritance#table-per-type-configuration)

---

**Última actualización**: 2025-10-10
**Próxima revisión**: Cuando se implemente la primera fase de mejoras
