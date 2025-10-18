# API Quick Reference

## 🔐 Autenticación

```bash
POST /api/users/login
```

```json
{
  "userName": "string",
  "password": "string"
}
```

**Token incluye:**
- `MemberId` (si es miembro) → usado automáticamente en academic endpoints

---

## 📚 Academic Management

### Registrar Grado Académico

```bash
POST /api/academic/degree-education
Content-Type: multipart/form-data
Authorization: Bearer {token}
```

**Form Fields:**
```
institutionId         (required, guid)
name                  (required, string)
academicDegree        (required, string)
major                 (required, string)
titleReceived         (required, string)
status                (required, string)
hasHonors             (required, boolean)
description           (optional, string)
specialization        (optional, string)
thesisTitle           (optional, string)
gpa                   (optional, 0-100)
startYear/Month/Day   (optional, int)
endYear/Month/Day     (optional, int)
document              (optional, file)
```

**Response:**
```json
{
  "wasSuccessful": true,
  "data": {
    "memberEducationId": "guid",
    "educationId": "guid",
    "educationName": "string",
    "institutionName": "string",
    "titleReceived": "string",
    "status": "string",
    "documentUrl": "string"
  }
}
```

---

## 🏢 Institution Management

### Registrar Tipo de Institución

```bash
POST /api/institution-types
Content-Type: application/json
Authorization: Bearer {token}
```

```json
{
  "name": "Universidad",
  "description": "Descripción opcional"
}
```

**Response:**
```json
{
  "wasSuccessful": true,
  "data": {
    "id": "guid",
    "name": "Universidad"
  }
}
```

### Registrar Institución

```bash
POST /api/institutions
Content-Type: application/json
Authorization: Bearer {token}
```

```json
{
  "name": "Universidad Nacional de Ingeniería",
  "institutionTypeId": "guid"
}
```

**Response:**
```json
{
  "wasSuccessful": true,
  "data": {
    "id": "guid",
    "name": "Universidad Nacional de Ingeniería",
    "institutionTypeName": "Universidad"
  }
}
```

---

## 📝 Códigos de Estado

| Código | Significado |
|--------|-------------|
| 200/201 | ✅ Success |
| 400 | ❌ Validación |
| 401 | ❌ No autenticado |
| 403 | ❌ Sin permisos |
| 404 | ❌ No encontrado |
| 409 | ❌ Duplicado |
| 500 | ❌ Error servidor |

---

## 🚀 Frontend Quick Start

### Fetch Example

```javascript
// Registrar grado académico con archivo
const formData = new FormData();
formData.append('institutionId', institutionId);
formData.append('name', 'Ingeniería Civil');
formData.append('academicDegree', 'Licenciatura');
formData.append('major', 'Ingeniería Civil');
formData.append('titleReceived', 'Ingeniero Civil');
formData.append('status', 'Completado');
formData.append('hasHonors', 'true');

if (file) formData.append('document', file);

const response = await fetch('/api/academic/degree-education', {
  method: 'POST',
  headers: { 'Authorization': `Bearer ${token}` },
  body: formData
});
```

### Axios Example

```javascript
// Crear institución
const response = await axios.post(
  '/api/institutions',
  {
    name: 'UPDS',
    institutionTypeId: typeId
  },
  {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  }
);
```

---

## ⚠️ Notas Importantes

1. **MemberId automático**: El endpoint de grado académico extrae el `memberId` del token JWT. NO lo envíes en el body.

2. **Multipart/form-data**: El endpoint de grado académico usa `multipart/form-data` (no JSON) para soportar archivos.

3. **Fechas flexibles**: Puedes enviar solo año, año+mes, o completo (año+mes+día).

4. **Archivos opcionales**: El campo `document` es opcional. Si no hay archivo, solo envía los otros campos.

5. **Orden de creación**:
   - 1️⃣ Crear tipo de institución
   - 2️⃣ Crear institución
   - 3️⃣ Registrar grado académico

6. **Blob Storage**: Los archivos se guardan en Azure Blob Storage con nombres únicos: `{memberId}_{guid}.{extension}`

---

## 📄 TypeScript Types

```typescript
// Academic
interface RegisterDegreeEducationDto {
  institutionId: string;
  name: string;
  academicDegree: string;
  major: string;
  titleReceived: string;
  status: string;
  hasHonors: boolean;
  description?: string;
  specialization?: string;
  thesisTitle?: string;
  gpa?: number;
  startYear?: number;
  startMonth?: number;
  startDay?: number;
  endYear?: number;
  endMonth?: number;
  endDay?: number;
}

// Institution
interface RegisterInstitutionDto {
  name: string;
  institutionTypeId: string;
}

interface RegisterInstitutionTypeDto {
  name: string;
  description?: string;
}

// Generic Response
interface ApiResponse<T> {
  wasSuccessful: boolean;
  message: string;
  data?: T;
  errors?: string[];
}
```

---

Ver documentación completa en: [API-ENDPOINTS.md](./API-ENDPOINTS.md)
