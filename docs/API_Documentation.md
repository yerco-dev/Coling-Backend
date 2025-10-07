# Documentación de API - Coling Backend

**Base URL:** `http://localhost:7047/api`

## Tabla de Contenidos
- [Estructura de Respuestas](#estructura-de-respuestas)
- [Códigos de Estado](#códigos-de-estado)
- [Autenticación](#autenticación)
- [Endpoints de Autenticación](#endpoints-de-autenticación)
- [Endpoints de Gestión de Miembros](#endpoints-de-gestión-de-miembros)

---

## Estructura de Respuestas

Todas las respuestas de la API siguen esta estructura estándar:

```json
{
  "wasSuccessful": true,
  "message": "Mensaje descriptivo",
  "data": { /* datos del resultado */ },
  "errors": ["lista", "de", "errores"]
}
```

### Campos:
- **wasSuccessful** (boolean): Indica si la operación fue exitosa
- **message** (string): Mensaje descriptivo del resultado
- **data** (object|null): Datos del resultado (puede ser null)
- **errors** (array|null): Lista de errores de validación (presente solo en errores)

---

## Códigos de Estado

| Código | Descripción |
|--------|-------------|
| 200    | OK - Operación exitosa |
| 400    | Bad Request - Error en los datos de entrada |
| 401    | Unauthorized - No autenticado |
| 403    | Forbidden - Sin permisos suficientes |
| 404    | Not Found - Recurso no encontrado |
| 409    | Conflict - Conflicto con el estado actual |
| 500    | Internal Server Error - Error del servidor |

---

## Autenticación

La API utiliza autenticación basada en JWT (JSON Web Tokens).

### Obtener un Token
Realiza login en el endpoint `/api/auth/login` para obtener un token JWT.

### Usar el Token
Incluye el token en el header `Authorization` de tus peticiones:

```
Authorization: Bearer {tu_token_jwt}
```

### Roles de Usuario
- **Administrador**: Acceso completo a todos los endpoints
- **Moderador**: Puede gestionar solicitudes de miembros
- **Miembro**: Usuario estándar con acceso limitado

---

## Endpoints de Autenticación

### 1. Registro de Miembro

**POST** `/api/auth/register-member`

Permite a un usuario registrarse como miembro. El registro queda en estado "Pendiente de Verificación" hasta que un administrador o moderador lo apruebe.

#### Headers
```
Content-Type: application/json
```

#### Request Body
```json
{
  "userName": "string",           // Requerido, máx. 256 caracteres
  "email": "string",              // Requerido, formato email válido, máx. 256 caracteres
  "password": "string",           // Requerido, 8-100 caracteres, debe incluir mayúscula, minúscula, número y carácter especial
  "nationalId": "string",         // Requerido, máx. 20 caracteres
  "firstNames": "string",         // Requerido, máx. 100 caracteres
  "paternalLastName": "string",   // Opcional, máx. 100 caracteres
  "maternalLastName": "string",   // Requerido, máx. 100 caracteres
  "membershipDate": "2024-01-15T00:00:00Z", // Requerido, formato ISO 8601
  "membershipCode": "string",     // Requerido, máx. 50 caracteres
  "titleNumber": "string"         // Requerido, máx. 100 caracteres
}
```

#### Validaciones de Password
- Mínimo 8 caracteres, máximo 100
- Al menos una letra mayúscula
- Al menos una letra minúscula
- Al menos un número
- Al menos un carácter especial (@$!%*?&#)

#### Response - Éxito (200)
```json
{
  "wasSuccessful": true,
  "message": "Miembro registrado exitosamente. Pendiente de aprobación.",
  "data": {
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userName": "string",
    "email": "string"
  },
  "errors": null
}
```

#### Response - Error de Validación (400)
```json
{
  "wasSuccessful": false,
  "message": "Errores de validación en el registro.",
  "data": null,
  "errors": [
    "La contraseña debe tener entre 8 y 100 caracteres.",
    "El email ya está en uso."
  ]
}
```

#### Response - Usuario Duplicado (409)
```json
{
  "wasSuccessful": false,
  "message": "El nombre de usuario ya existe.",
  "data": null,
  "errors": null
}
```

---

### 2. Inicio de Sesión

**POST** `/api/auth/login`

Permite a un usuario autenticarse y obtener un token JWT.

#### Headers
```
Content-Type: application/json
```

#### Request Body
```json
{
  "userName": "string",    // Requerido
  "password": "string"     // Requerido
}
```

#### Response - Éxito (200)
```json
{
  "wasSuccessful": true,
  "message": "Inicio de sesión exitoso.",
  "data": {
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "personId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userName": "admin",
    "email": "admin@coling.com",
    "firstNames": "Administrador",
    "fullName": "Administrador Sistema",
    "role": "Administrador",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  },
  "errors": null
}
```

#### Response - Credenciales Inválidas (404)
```json
{
  "wasSuccessful": false,
  "message": "Usuario o contraseña incorrectos.",
  "data": null,
  "errors": null
}
```

#### Response - Cuenta Desactivada (409)
```json
{
  "wasSuccessful": false,
  "message": "La cuenta está desactivada.",
  "data": null,
  "errors": null
}
```

---

## Endpoints de Gestión de Miembros

### 3. Obtener Miembros Pendientes

**GET** `/api/members/pending`

Obtiene la lista de miembros que están pendientes de aprobación.

#### Autenticación
**Requerida** - Roles permitidos: `Administrador`, `Moderador`

#### Headers
```
Authorization: Bearer {token}
```

#### Response - Éxito (200)
```json
{
  "wasSuccessful": true,
  "message": "Miembros pendientes obtenidos exitosamente.",
  "data": [
    {
      "memberId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "personId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "userName": "testmember1",
      "email": "testmember1@example.com",
      "isActive": true,
      "nationalId": "12345678",
      "firstNames": "Test",
      "paternalLastName": "Member",
      "maternalLastName": "One",
      "fullName": "Test Member One",
      "birthDate": "1990-01-15T00:00:00Z",
      "photoUrl": null,
      "membershipDate": "2024-01-15T00:00:00Z",
      "membershipCode": "MEM-2024-001",
      "titleNumber": "TIT-001",
      "status": "Pendiente de Verificación"
    }
  ],
  "errors": null
}
```

#### Response - Sin Autenticación (401)
```json
{
  "wasSuccessful": false,
  "message": "Se requiere autenticación",
  "data": null,
  "errors": null
}
```

#### Response - Sin Permisos (403)
```json
{
  "wasSuccessful": false,
  "message": "No tiene permisos para acceder a este recurso",
  "data": null,
  "errors": null
}
```

---

### 4. Aprobar Miembro

**POST** `/api/members/approve`

Aprueba un miembro que está en estado pendiente y le asigna el rol de "Miembro".

#### Autenticación
**Requerida** - Roles permitidos: `Administrador`, `Moderador`

#### Headers
```
Authorization: Bearer {token}
Content-Type: application/json
```

#### Request Body
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"  // Requerido
}
```

#### Response - Éxito (200)
```json
{
  "wasSuccessful": true,
  "message": "Miembro aprobado exitosamente.",
  "data": {
    "memberId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "personId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userName": "testmember1",
    "email": "testmember1@example.com",
    "isActive": true,
    "nationalId": "12345678",
    "firstNames": "Test",
    "paternalLastName": "Member",
    "maternalLastName": "One",
    "fullName": "Test Member One",
    "birthDate": "1990-01-15T00:00:00Z",
    "photoUrl": null,
    "membershipDate": "2024-01-15T00:00:00Z",
    "membershipCode": "MEM-2024-001",
    "titleNumber": "TIT-001",
    "status": "Verificado"
  },
  "errors": null
}
```

#### Response - Usuario No Encontrado (404)
```json
{
  "wasSuccessful": false,
  "message": "Usuario no encontrado.",
  "data": null,
  "errors": null
}
```

#### Response - Ya Procesado (409)
```json
{
  "wasSuccessful": false,
  "message": "El miembro ya ha sido procesado anteriormente.",
  "data": null,
  "errors": null
}
```

#### Response - Sin Autenticación (401)
```json
{
  "wasSuccessful": false,
  "message": "Se requiere autenticación",
  "data": null,
  "errors": null
}
```

#### Response - Sin Permisos (403)
```json
{
  "wasSuccessful": false,
  "message": "No tiene permisos para acceder a este recurso",
  "data": null,
  "errors": null
}
```

---

### 5. Rechazar Miembro

**POST** `/api/members/reject`

Rechaza un miembro que está en estado pendiente y desactiva su cuenta.

#### Autenticación
**Requerida** - Roles permitidos: `Administrador`, `Moderador`

#### Headers
```
Authorization: Bearer {token}
Content-Type: application/json
```

#### Request Body
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",  // Requerido
  "reason": "Información proporcionada no coincide con registros oficiales"  // Opcional, máx. 500 caracteres
}
```

#### Response - Éxito (200)
```json
{
  "wasSuccessful": true,
  "message": "Miembro rechazado exitosamente.",
  "data": {
    "memberId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "personId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userName": "testmember1",
    "email": "testmember1@example.com",
    "isActive": false,
    "nationalId": "12345678",
    "firstNames": "Test",
    "paternalLastName": "Member",
    "maternalLastName": "One",
    "fullName": "Test Member One",
    "birthDate": "1990-01-15T00:00:00Z",
    "photoUrl": null,
    "membershipDate": "2024-01-15T00:00:00Z",
    "membershipCode": "MEM-2024-001",
    "titleNumber": "TIT-001",
    "status": "Suspendido"
  },
  "errors": null
}
```

#### Response - Usuario No Encontrado (404)
```json
{
  "wasSuccessful": false,
  "message": "Usuario no encontrado.",
  "data": null,
  "errors": null
}
```

#### Response - Ya Procesado (409)
```json
{
  "wasSuccessful": false,
  "message": "El miembro ya ha sido procesado anteriormente.",
  "data": null,
  "errors": null
}
```

#### Response - Sin Autenticación (401)
```json
{
  "wasSuccessful": false,
  "message": "Se requiere autenticación",
  "data": null,
  "errors": null
}
```

#### Response - Sin Permisos (403)
```json
{
  "wasSuccessful": false,
  "message": "No tiene permisos para acceder a este recurso",
  "data": null,
  "errors": null
}
```

---

## Ejemplos de Integración

### JavaScript (Fetch API)

#### Login
```javascript
async function login(userName, password) {
  const response = await fetch('http://localhost:7047/api/auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ userName, password }),
  });

  const result = await response.json();

  if (result.wasSuccessful) {
    // Guardar token para uso posterior
    localStorage.setItem('token', result.data.token);
    return result.data;
  } else {
    throw new Error(result.message);
  }
}
```

#### Obtener Miembros Pendientes
```javascript
async function getPendingMembers() {
  const token = localStorage.getItem('token');

  const response = await fetch('http://localhost:7047/api/members/pending', {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
    },
  });

  const result = await response.json();

  if (result.wasSuccessful) {
    return result.data;
  } else {
    throw new Error(result.message);
  }
}
```

#### Aprobar Miembro
```javascript
async function approveMember(userId) {
  const token = localStorage.getItem('token');

  const response = await fetch('http://localhost:7047/api/members/approve', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ userId }),
  });

  const result = await response.json();

  if (result.wasSuccessful) {
    return result.data;
  } else {
    throw new Error(result.message);
  }
}
```

### TypeScript (Axios)

```typescript
import axios, { AxiosInstance } from 'axios';

// Tipos
interface ApiResponse<T> {
  wasSuccessful: boolean;
  message: string;
  data: T | null;
  errors: string[] | null;
}

interface LoginRequest {
  userName: string;
  password: string;
}

interface LoginResponse {
  userId: string;
  personId: string;
  userName: string;
  email: string;
  firstNames: string;
  fullName: string;
  role: string;
  token: string;
}

interface MemberDetails {
  memberId: string;
  userId: string;
  personId: string;
  userName: string;
  email: string;
  isActive: boolean;
  nationalId: string;
  firstNames: string;
  paternalLastName: string | null;
  maternalLastName: string;
  fullName: string;
  birthDate: string | null;
  photoUrl: string | null;
  membershipDate: string;
  membershipCode: string;
  titleNumber: string;
  status: string;
}

// Cliente API
class ColingApiClient {
  private client: AxiosInstance;

  constructor(baseURL: string = 'http://localhost:7047/api') {
    this.client = axios.create({
      baseURL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Interceptor para agregar token automáticamente
    this.client.interceptors.request.use((config) => {
      const token = localStorage.getItem('token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });
  }

  async login(userName: string, password: string): Promise<LoginResponse> {
    const response = await this.client.post<ApiResponse<LoginResponse>>(
      '/auth/login',
      { userName, password }
    );

    if (response.data.wasSuccessful && response.data.data) {
      localStorage.setItem('token', response.data.data.token);
      return response.data.data;
    }

    throw new Error(response.data.message);
  }

  async getPendingMembers(): Promise<MemberDetails[]> {
    const response = await this.client.get<ApiResponse<MemberDetails[]>>(
      '/members/pending'
    );

    if (response.data.wasSuccessful && response.data.data) {
      return response.data.data;
    }

    throw new Error(response.data.message);
  }

  async approveMember(userId: string): Promise<MemberDetails> {
    const response = await this.client.post<ApiResponse<MemberDetails>>(
      '/members/approve',
      { userId }
    );

    if (response.data.wasSuccessful && response.data.data) {
      return response.data.data;
    }

    throw new Error(response.data.message);
  }

  async rejectMember(userId: string, reason?: string): Promise<MemberDetails> {
    const response = await this.client.post<ApiResponse<MemberDetails>>(
      '/members/reject',
      { userId, reason }
    );

    if (response.data.wasSuccessful && response.data.data) {
      return response.data.data;
    }

    throw new Error(response.data.message);
  }
}

export default ColingApiClient;
```

---

## Notas Importantes

### Manejo de Errores
- Siempre verifica el campo `wasSuccessful` antes de procesar `data`
- El campo `errors` contiene detalles adicionales cuando hay errores de validación
- Los mensajes en `message` están en español para facilitar su uso en la UI

### Tokens JWT
- Los tokens tienen una validez de 24 horas
- Después de expirar, el usuario debe hacer login nuevamente
- Guarda el token de forma segura (localStorage, sessionStorage, o cookies HTTP-only)

### Formato de Fechas
- Todas las fechas usan formato ISO 8601 (UTC)
- Ejemplo: `2024-01-15T00:00:00Z`

### GUIDs
- Todos los IDs son GUIDs en formato string
- Ejemplo: `3fa85f64-5717-4562-b3fc-2c963f66afa6`

### Estados de Miembro
- **Pendiente de Verificación**: Registro nuevo, esperando aprobación
- **Verificado**: Miembro aprobado y activo
- **Suspendido**: Miembro rechazado o suspendido

---

## Soporte

Para reportar problemas o solicitar nuevas funcionalidades, contacta al equipo de desarrollo.
