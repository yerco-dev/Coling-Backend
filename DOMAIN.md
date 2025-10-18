# Análisis del Dominio - Coling Backend

## Resumen Ejecutivo

**Coling** es un sistema de gestión de membresía para el Colegio de Ingenieros que maneja información académica, profesional y de experiencia laboral de sus miembros. El sistema utiliza Clean Architecture y sigue el patrón Party Model para representar entidades legales (personas/instituciones).

---

## 1. Entidades Base

### 1.1 BaseEntity

- **Propósito**: Clase base abstracta para todas las entidades del sistema
- **Propiedades**:
  - `Id` (Guid): Identificador único
  - `IsActive` (bool): Soft delete flag
- **Ubicación**: `Coling.Domain.Entities.Base.BaseEntity`

### 1.2 NamedEntity

- **Hereda de**: BaseEntity
- **Propósito**: Entidades que tienen nombre
- **Propiedades adicionales**:
  - `Name` (string, max 100): Nombre de la entidad

### 1.3 Party (Patrón Party Model)

- **Hereda de**: BaseEntity
- **Propósito**: Patrón de diseño que representa cualquier entidad legal (persona u organización)
- **Hijos**: Person, Institution
- **Justificación**: Permite abstraer conceptos compartidos entre personas e instituciones

---

## 2. Gestión de Identidad y Usuarios

### 2.1 User (extends IdentityUser<Guid>)

- **Propósito**: Credenciales de autenticación en el sistema
- **Relaciones**:
  - `PersonId` (Guid, Required): FK a Person (1:1)
  - Navega a `Person`
- **Propiedades adicionales**:
  - `IsActive` (bool): Control de acceso
- **Características**:
  - Usa ASP.NET Core Identity con Guid como PK
  - Un User siempre debe tener una Person asociada
  - Una Person puede o no tener User (no todos necesitan login)

### 2.2 Role (extends IdentityRole<Guid>)

- **Propósito**: Roles del sistema
- **Propiedades**:
  - `Name` (string, Required): Nombre del rol en español
  - `Description` (string?, max 250): Descripción del rol
  - `IsActive` (bool): Control de soft delete
- **Roles del Sistema**:
  - "Administrador" (Admin)
  - "Moderador" (Moderator)
  - "Miembro" (Member)

---

## 3. Gestión de Personas y Miembros

### 3.1 Person (extends Party)

- **Propósito**: Información personal de un individuo
- **Propiedades**:
  - `NationalId` (string, max 20, Required, **UNIQUE**): Cédula/DNI/Identificación nacional
  - `FirstNames` (string, max 100, Required): Nombres
  - `PaternalLastName` (string?, max 100): Apellido paterno
  - `MaternalLastName` (string, max 100, Required): Apellido materno
  - `BirthDate` (DateTime?, DataType.Date): Fecha de nacimiento
  - `PhotoUrl` (string?, max 500, Url): URL de foto de perfil (blob storage)
- **Relaciones**:
  - `Member` (1:0..1): Navegación opcional a Member
- **Propiedades Computadas**:
  - `FullName` (NotMapped): Concatenación de nombres completos
- **Validaciones Importantes**:
  - NationalId debe ser único en toda la base de datos

**PREGUNTA 1**: ¿El apellido paterno es opcional porque consideran casos de personas con un solo apellido, o hay otra razón?
R. si, normalmente es la madre la que registra a su hijo, pero puede que en un futuro se dejen los dos apellidos opcionales

### 3.2 Member (extends BaseEntity)

- **Propósito**: Representa un miembro activo o pendiente del Colegio de Ingenieros
- **Propiedades**:
  - `MembershipDate` (DateTime, Required, DataType.Date): Fecha de ingreso como miembro
  - `MembershipCode` (string, max 50, Required, **UNIQUE**): Código único de membresía
  - `TitleNumber` (string, max 100, Required, **UNIQUE**): Número de título profesional
  - `Status` (string, max 50, Required): Estado del miembro
    - "Pendiente de Verificación" (Pending)
    - "Verificado" (Verified)
    - "Suspendido" (Suspended)
  - `PersonId` (Guid, Required): FK a Person (1:1)
- **Relaciones**:
  - `Person` (1:1, Required): Navegación a Person
  - `Educations` (1:N): Colección de MemberEducation
  - `WorkExperiences` (1:N): Colección de WorkExperience
- **Reglas de Negocio**:
  - Un miembro en estado "Pendiente de Verificación" NO puede hacer login
  - Un miembro "Verificado" obtiene automáticamente el rol "Miembro"
  - Un miembro "Suspendido" tiene su cuenta desactivada (IsActive = false)

**PREGUNTA 2**: ¿Qué diferencia hay entre MembershipCode y TitleNumber? ¿El TitleNumber es el número de colegiatura oficial emitido por el gobierno/autoridad reguladora?
R. MembershipCode es el codigo del afiliado que le entrega la institución y debe tener un titulo de ingeniero para ser afiliado del que tambien tiene su numero único

**PREGUNTA 3**: ¿Es posible que un Member sea "Suspendido" temporalmente y luego reactivado, o es un estado final?
R. es posible que sea suspendido por que no ha pagado, en un futurio habrán más estados

---

## 4. Gestión Académica

### 4.1 Education (abstract class, extends NamedEntity)

- **Propósito**: Clase base abstracta para todas las formaciones académicas (marcada como `abstract`)
- **Propiedades**:
  - `InstitutionId` (Guid, Required): FK a Institution
  - `Description` (string?, max 500): Descripción de la educación
  - `Name` (string, max 100, heredado): Nombre del programa/curso/certificación
- **Relaciones**:
  - `Institution`: Navegación a Institution
- **Hijos**: DegreeEducation, ContinuingEducation, ProfessionalCertification

**OBSERVACIÓN**: Education es una entidad de catálogo/plantilla, mientras que MemberEducation es la instancia específica de un miembro que cursó esa educación.

### 4.2 DegreeEducation (extends Education)

- **Propósito**: Educación formal con grado académico (licenciatura, maestría, etc.)
- **Propiedades**:
  - `AcademicDegree` (string, max 50, Required): Tipo de grado académico
    - "Bachiller" (BachelorDegree)
    - "Licenciatura" (Licentiate)
    - "Diplomado" (Diploma)
    - "Especialidad" (Specialty)
    - "Maestría" (Master)
    - "Doctorado" (Doctorate)
    - "Postdoctorado" (Postdoctorate)
  - `Major` (string, max 200, Required): Carrera/especialidad principal
  - `Specialization` (string?, max 100): Sub-especialización
  - `ThesisTitle` (string?, max 200): Título de tesis
  - `GPA` (decimal?, Range 0-100, decimal(5,2)): Promedio de calificaciones (escala 0-100)
  - `HasHonors` (bool): Indicador de honores/mención honorífica

**PREGUNTA 4**: ¿El campo `Name` heredado de NamedEntity se usa para el nombre del programa? Por ejemplo: "Ingeniería Civil" en Name y "Civil" en Major? ¿O se duplica?
R. aqui me gustaría que me dieras tu interpretación, de que sería bueno registrar.

### 4.3 ContinuingEducation (extends Education)

- **Propósito**: Educación continua (cursos, talleres, seminarios, conferencias)
- **Propiedades**:
  - `DurationHours` (int?, Range 1-10000): Duración en horas
  - `EducationType` (string, max 50, Required): Tipo de educación continua
    - "Curso" (Course)
    - "Taller" (Workshop)
    - "Seminario" (Seminar)
    - "Conferencia" (Conference)
  - `IssuesCertificate` (bool): Si emite certificado
  - `CertificateNumber` (string?, max 100): Número de certificado si aplica

### 4.4 ProfessionalCertification (extends Education)

- **Propósito**: Certificaciones profesionales especializadas
- **Propiedades**:
  - `CertificationNumber` (string, max 100, Required): Número de certificación
  - `ExpirationDate` (DateTime?, DataType.Date): Fecha de expiración
  - `RequiresRenewal` (bool): Si requiere renovación

**PREGUNTA 5**: ¿ProfessionalCertification es para certificaciones como PMP, Six Sigma, certificaciones de fabricantes (Microsoft, AWS, etc.)? ¿O también incluye colegiatura en otros países?
R. Tengo la intención de que sea para certificaciones como de microsoft o aws o cisco.

### 4.5 MemberEducation (extends BaseEntity)

- **Propósito**: Relación many-to-many entre Member y Education con metadatos
- **Propiedades**:
  - `TitleReceived` (string, max 200, Required): Título específico recibido
  - `StartDate` (PartialDate?): Fecha de inicio
  - `EndDate` (PartialDate?): Fecha de finalización
  - `DocumentUrl` (string?, max 500, Url): URL del documento probatorio (blob storage)
  - `Status` (string, max 50, Required): Estado de la educación
    - "En progreso" (InProgress)
    - "Completado" (Completed)
    - "Abandonado" (DroppedOut)
  - `MemberId` (Guid, Required): FK a Member
  - `EducationId` (Guid, Required): FK a Education
- **Relaciones**:
  - `Member`: Navegación a Member
  - `Education`: Navegación a Education
- **Propiedades Computadas**:
  - `IsInProgress` (NotMapped): True si EndDate es null

**PREGUNTA 6**: ¿El flujo es: (1) Admin crea catálogo de Education, (2) Miembro asocia Education a su perfil mediante MemberEducation? ¿O los miembros pueden crear sus propias Education on-the-fly?
he pensado que agregar una clase clase base extra que tenga un campo para toda entidad que requiera verificación de un moderador o administrador, por ahora los afiliados los pueden crear.

**OBSERVACIÓN**: El uso de PartialDate es excelente para fechas académicas donde a veces solo conocemos año o mes/año.

---

## 5. Gestión de Instituciones

### 5.1 Institution (extends Party, implements INamedEntity)

- **Propósito**: Universidades, empresas, organizaciones donde se estudió o trabajó
- **Propiedades**:
  - `Name` (string, max 100, Required): Nombre de la institución
  - `InstitutionTypeId` (Guid, Required): FK a InstitutionType
- **Relaciones**:
  - `InstitutionType`: Tipo de institución
  - `WorkExperiences` (1:N): Experiencias laborales en esta institución

**OBSERVACIÓN**: Institution extiende Party, lo cual tiene sentido según el Party Model (las organizaciones son "parties" al igual que las personas).

**PREGUNTA 7**: ¿Las instituciones también pueden aparecer como Educations? Por ejemplo, ¿una Institution podría impartir múltiples DegreeEducation? Si es así, falta la relación Education.InstitutionId → Institution.

**RESPUESTA ENCONTRADA**: Sí, Education tiene `InstitutionId`, así que el modelo está completo. Una Institution puede ofrecer múltiples programas Education y también emplear personas (WorkExperience).

### 5.2 InstitutionType (extends NamedEntity)

- **Propósito**: Clasificación de instituciones
- **Ejemplos esperados**: "Universidad", "Instituto Técnico", "Empresa Privada", "Institución Pública", "ONG", etc.
- **Relaciones**:
  - `Institutions` (1:N): Instituciones de este tipo

**PREGUNTA 8**: ¿Tienen ya definidos los InstitutionTypes que van a usar, o los administradores pueden crear nuevos tipos?
R. los administradores podrán crear los tipos

---

## 6. Gestión de Experiencia Laboral

### 6.1 WorkExperience (extends BaseEntity)

- **Propósito**: Experiencia laboral de un miembro
- **Propiedades**:
  - `MemberId` (Guid, Required): FK a Member
  - `InstitutionId` (Guid, Required): FK a Institution donde trabajó
  - `JobTitle` (string, max 150, Required): Cargo/puesto
  - `StartDate` (PartialDate, Required): Fecha de inicio
  - `EndDate` (PartialDate?): Fecha de fin (null si es trabajo actual)
  - `Description` (string?, max 2000): Descripción del trabajo
  - `Responsibilities` (string?, max 1000): Responsabilidades
  - `Achievements` (string?, max 1000): Logros
  - `DocumentUrl` (string?, max 500, Url): Documento probatorio (blob storage)
- **Relaciones**:
  - `Member`: Miembro dueño de la experiencia
  - `Institution`: Institución empleadora
  - `WorkExperienceFields` (1:N): Campos de trabajo relacionados
- **Propiedades Computadas**:
  - `DurationInMonths` (NotMapped): Cálculo de duración en meses

**OBSERVACIÓN**: Excelente uso de PartialDate para fechas laborales. Muchas veces solo recordamos mes/año de inicio/fin.

**NOTA**: El campo `IsCurrentJob` fue eliminado. La lógica de trabajo actual se determina cuando `EndDate` es null.

### 6.2 WorkField (extends NamedEntity)

- **Propósito**: Campo/área de trabajo específico
- **Ejemplos**: "Estructuras", "Hidráulica", "Gestión de Proyectos", "Supervisión de Obra", etc.
- **Propiedades**:
  - `Description` (string?, max 500): Descripción del campo
  - `WorkFieldCategoryId` (Guid?): FK opcional a WorkFieldCategory
- **Relaciones**:
  - `WorkFieldCategory`: Categoría padre
  - `WorkExperienceFields` (1:N): Experiencias en este campo

**OBSERVACIÓN**: WorkFieldCategoryId es nullable, lo que permite WorkFields sin categoría.
Aqui quiero que todos los campos esten categorizados, ya he hecho los cambios necesarios quitandole el nullable y agregadole required

### 6.3 WorkFieldCategory (extends NamedEntity)

- **Propósito**: Categoría de alto nivel para clasificar WorkFields
- **Ejemplos**: "Ingeniería Civil", "Ingeniería Eléctrica", "Gestión", "Consultoría"
- **Propiedades**:
  - `Description` (string?, max 500): Descripción de la categoría
- **Relaciones**:
  - `WorkFields` (1:N): Campos en esta categoría

**OBSERVACIÓN**: Esta es una jerarquía de 2 niveles:

```
WorkFieldCategory (ej: "Ingeniería Civil")
  └─ WorkField (ej: "Estructuras", "Hidráulica")
      └─ WorkExperienceField (ej: Juan trabajó en Estructuras en su empleo en ABC Corp)
```

**PREGUNTA 10**: ¿Consideraron permitir jerarquías más profundas (subcategorías), o 2 niveles son suficientes para su dominio?
R. dos niveles son suficientes para mi dominio

### 6.4 WorkExperienceField (extends BaseEntity)

- **Propósito**: Tabla de unión many-to-many entre WorkExperience y WorkField
- **Propiedades**:
  - `WorkExperienceId` (Guid, Required): FK a WorkExperience
  - `WorkFieldId` (Guid, Required): FK a WorkField
- **Relaciones**:
  - `WorkExperience`: Experiencia laboral
  - `WorkField`: Campo de trabajo

**OBSERVACIÓN**: Permite que una WorkExperience esté asociada a múltiples WorkFields (ej: un Ingeniero de Proyectos puede trabajar en "Gestión", "Supervisión" y "Estructuras" en el mismo empleo).

---

## 7. Value Objects

### 7.1 PartialDate

- **Propósito**: Representar fechas incompletas (solo año, año/mes, o año/mes/día)
- **Propiedades**:
  - `Year` (int): Año (1900-2100)
  - `Month` (int?): Mes opcional (1-12)
  - `Day` (int?): Día opcional (1-31)
- **Métodos**:
  - `IsComplete`: True si tiene día y mes
  - `ToApproximateDate()`: Convierte a DateTime usando valores mínimos para partes faltantes
  - `ToMinDate()`: Fecha mínima posible
  - `ToMaxDate()`: Fecha máxima posible
  - `CompareTo()`: Permite comparación de fechas parciales
  - `ToString()`: Formato "DD/MM/YYYY", "MM/YYYY" o "YYYY"
- **Validaciones**:
  - Valida días según mes (28/29/30/31)
  - Considera años bisiestos

**OBSERVACIÓN**: Excelente implementación de Value Object. Inmutable (setters privados) y con validación en constructor.

**PREGUNTA 11**: ¿Cómo se mapea PartialDate a la base de datos? ¿Como tres columnas separadas (Year, Month, Day) o como un tipo complejo/JSON?
quiero que se mapeen como tres columnas separadas para cada ves que se agregue un PartialDate a una entidad

### 7.2 DateRange

- **Propósito**: Rango de fechas con inicio y fin opcional
- **Propiedades**:
  - `StartDate` (PartialDate, Required): Fecha de inicio
  - `EndDate` (PartialDate?): Fecha de fin opcional
- **Métodos**:
  - `IsCurrent`: True si EndDate es null
  - `ApproximateDurationInMonths`: Cálculo de duración
- **Validaciones**:
  - EndDate debe ser posterior a StartDate

**OBSERVACIÓN**: DateRange parece diseñado para usar en lugar de tener StartDate/EndDate en cada entidad, pero veo que MemberEducation y WorkExperience usan PartialDate directamente en lugar de DateRange. ¿Es DateRange parte de un diseño futuro?

**PREGUNTA 12**: ¿DateRange está en uso actualmente o es un Value Object preparado para refactorización futura?
R. No se usa actualmente, ya lo he quitado

---

## 8. Wrappers y Tipos de Retorno

### 8.1 ActionResponse<T>

- **Propósito**: Wrapper estandarizado para respuestas de operaciones
- **Propiedades**:
  - `WasSuccessful` (bool): Indicador de éxito
  - `Message` (string?): Mensaje descriptivo
  - `ResultCode` (ResultCode): Código HTTP-like
  - `Result` (T?): Datos de retorno
  - `Errors` (List<string>?): Lista de errores
- **Métodos Factory**:
  - `Success(T result, string? message)`
  - `Failure(string message, ResultCode code)`
  - `NotFound(string message)`
  - `Conflict(string message)`
- **ResultCodes**:
  - Ok = 200
  - NotFound = 404
  - DatabaseError = 500
  - InputError = 400
  - Conflict = 409
  - Unauthorized = 401
  - Forbidden = 403

**OBSERVACIÓN**: Excelente patrón Result para manejar errores de forma funcional sin excepciones.

---

## 9. Constantes de Negocio

### BusinessConstants

Proporciona diccionarios de mapeo entre enums y valores en español:

**Métodos Genéricos**:

- `IsValidConstant<TEnum>()`: Valida si un string es un valor válido
- `ParseConstant<TEnum>()`: Convierte string a enum

**Diccionarios**:

- `MemberStatusValues`
- `SystemRolesValues`
- `EducationStatusValues`
- `AcademicDegreeValues`
- `ContinuingEducationTypeValues`

**OBSERVACIÓN**: Buen patrón para i18n y mantener consistencia en valores de negocio.

---

## 10. Relaciones y Cardinalidades

### Diagrama Conceptual

```
User (1) ──────────── (1) Person (1) ──────────── (0..1) Member
                         │                           │
                         │                           ├──── (N) MemberEducation ──── (1) Education
                         │                           │         └── (DegreeEducation, ContinuingEducation, ProfessionalCertification)
                         │                           │
                         │                           └──── (N) WorkExperience ──── (1) Institution ──── (1) InstitutionType
                         │                                      │
                         │                                      └──── (N) WorkExperienceField ──── (1) WorkField ──── (0..1) WorkFieldCategory
                         │
                         └── (Party)

Institution extends Party ──── (1) InstitutionType
```

---

## 11. Reglas de Negocio Identificadas

### Validaciones de Unicidad

1. `Person.NationalId` - UNIQUE
2. `Member.MembershipCode` - UNIQUE
3. `Member.TitleNumber` - UNIQUE
4. `User.Email` - UNIQUE (por Identity)
5. `User.UserName` - UNIQUE (por Identity)

### Workflow de Registro de Miembro

1. Usuario se registra → crea User + Person + Member (Status: "Pendiente de Verificación")
2. Admin/Moderador aprueba → Member.Status = "Verificado" + asigna rol "Miembro"
3. Admin/Moderador rechaza → Member.Status = "Suspendido" + IsActive = false

### Restricciones de Login

- Usuarios con Member.Status = "Pendiente de Verificación" NO pueden hacer login
- Usuarios con IsActive = false NO pueden hacer login

### Soft Delete

- Todas las entidades usan `IsActive` para soft delete
- Roles también tienen `IsActive`

---

## 12. Preguntas y Dudas Pendientes

### Preguntas de Implementación

13. ¿Ya existe seeder para WorkFieldCategory y WorkField?
    Aún no exsite
14. ¿Ya existe seeder para InstitutionType?
    aun no existe
15. ¿Se van a crear endpoints CRUD para todas estas entidades?
    iremos creando casos de uso relacionados a estas entidades, pero repositorios para cruds si.
16. ¿Hay validación de que DocumentUrl apunte a blob storage válido?
    Aún no, sería interesante agregarlo más adelante
17. ¿Se implementará versionado de documentos (ej: nueva versión de título)?
    no por ahora
18. ¿Hay plan de auditoría (CreatedAt, CreatedBy, ModifiedAt, ModifiedBy)?
    no por ahora

### Preguntas de Reglas de Negocio

19. ¿Puede un Member tener múltiples TitleNumber (títulos de diferentes carreras)?
    si, pero solo usará uno para registrarse y es el que esta asociado a su entidad afiliado
20. ¿Un miembro puede estar "Verificado" sin tener ninguna MemberEducation?
    si, pues se habría verificado que sea un afiliado y que tenga un titulo cuyo nombre se ha asociado
21. ¿Se validará que un miembro tenga al menos un título de ingeniería?
    no por ahora, los titulos registrados serán usados para construir su curriculum y esto creo que me traerá problemas más adelante
22. ¿Los campos de trabajo (WorkField) serán de selección múltiple obligatoria en WorkExperience?
    uno podrá agregar 0, 1 o muchos workfield a una workexperience
23. ¿Habrá límite de WorkExperience o MemberEducation por miembro?
    no

---

## 13. Recomendaciones

### Mejoras Sugeridas

1. **Auditoría**: Agregar `CreatedAt`, `CreatedBy`, `ModifiedAt`, `ModifiedBy` a BaseEntity
2. **Soft Delete Consistency**: Considerar `DeletedAt` y `DeletedBy` en lugar de solo `IsActive`
3. **Validación Documentos**: Agregar validación de que DocumentUrl sea válido
4. **Enum Persistence**: Considerar almacenar enums como enteros en BD y mapear en aplicación
5. **Institution Address**: Considerar agregar dirección/país a Institution
6. **Person Contact**: Considerar agregar teléfono/email de contacto a Person
7. **Member Photo**: El PhotoUrl está en Person, ¿debería también/en su lugar estar en Member?
   la mayoría de estos puntos se analizarán y agregarán en un futuro

### Documentación Pendiente

1. Crear diagramas ER de base de datos
2. Documentar flujos de aprobación de miembros
3. Documentar validaciones de negocio en CLAUDE.md
4. Agregar ejemplos de datos semilla para testing

---

## 14. Data Annotations Aplicadas

### Resumen de Validaciones Básicas

Se han aplicado las siguientes data annotations básicas a las entidades del dominio:

#### **Validaciones de URL** (`[Url]`)
- `Person.PhotoUrl` (max 500): URL de foto de perfil en blob storage
- `MemberEducation.DocumentUrl` (max 500): URL de documento probatorio en blob storage
- `WorkExperience.DocumentUrl` (max 500): URL de documento probatorio en blob storage

#### **Validaciones de Rango** (`[Range]`)
- `DegreeEducation.GPA`: Range(0, 100) - Escala boliviana 0-100
  - También incluye `[Column(TypeName = "decimal(5,2)")]` para precisión en BD
- `ContinuingEducation.DurationHours`: Range(1, 10000) - Evita valores negativos o inválidos

#### **Validaciones de Fecha** (`[DataType(DataType.Date)]`)
- `Person.BirthDate`: Fecha de nacimiento (solo fecha, sin hora)
- `Member.MembershipDate`: Fecha de ingreso como miembro
- `ProfessionalCertification.ExpirationDate`: Fecha de expiración de certificación

#### **Cambios Estructurales**
- `Education`: Marcada como clase `abstract` (no permite instancias directas)
- `Member.TitleNumber`: Agregado `[Required]` (inconsistencia corregida)
- `WorkExperience.IsCurrentJob`: **ELIMINADO** - se determina por `EndDate == null`

### Convenciones de EF Core Utilizadas

El proyecto **NO** utiliza atributos `[ForeignKey]` explícitos, confiando en las convenciones de Entity Framework Core para detectar:
- Propiedades `{Entidad}Id` como foreign keys
- Propiedades de navegación asociadas

### Próximos Pasos

En futuras iteraciones se agregarán:
1. Validaciones de negocio más profundas (FluentValidation)
2. Índices únicos compuestos
3. Validación de documentos en blob storage
4. Reglas de validación cross-property (ej: EndDate >= StartDate)

---

## 15. Conclusión

El dominio está **muy bien diseñado** con:

- ✅ Uso correcto de Clean Architecture
- ✅ Patrón Party Model aplicado apropiadamente
- ✅ Value Objects inmutables (PartialDate)
- ✅ Separación clara entre catálogos (Education, WorkField) y datos de miembros (MemberEducation, WorkExperience)
- ✅ Relaciones many-to-many bien modeladas
- ✅ Soft delete consistente
- ✅ Localización en español coherente
- ✅ Data annotations básicas aplicadas para validaciones de primer nivel

**Áreas que requieren clarificación** (ver Preguntas arriba):

- Algunas reglas de negocio no están totalmente claras
- Persistencia de Value Objects
- Workflow completo de algunos procesos

---

**Fecha de análisis inicial**: 2025-10-10
**Última actualización**: 2025-10-10 (Data Annotations aplicadas)
**Analista**: Claude (Sonnet 4.5)
**Próxima revisión**: Validaciones de negocio profundas (FluentValidation)
