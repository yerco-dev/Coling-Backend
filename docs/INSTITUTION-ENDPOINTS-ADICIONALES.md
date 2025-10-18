# Endpoints Adicionales de Institution Management

## Institution Types - Endpoints GET, PUT, DELETE

### 2. Listar Todos los Tipos de Institución

Obtiene todos los tipos de institución registrados en el sistema.

**Endpoint:** `GET /api/institution-type`

**Método:** GET

**Autenticación:** Requerida

**Request Example:**
```javascript
const response = await fetch('http://localhost:7071/api/institution-type', {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${token}`
  }
});
```

**Response Success (200 OK):**
```json
{
  "wasSuccessful": true,
  "message": "Tipos de institución obtenidos correctamente.",
  "data": [
    {
      "id": "guid",
      "name": "Universidad",
      "isActive": true
    },
    {
      "id": "guid",
      "name": "Empresa",
      "isActive": true
    },
    {
      "id": "guid",
      "name": "Instituto Técnico",
      "isActive": true
    }
  ],
  "errors": null
}
```

---

### 3. Obtener Tipo de Institución por ID

Obtiene un tipo de institución específico por su ID.

**Endpoint:** `GET /api/institution-type/{id}`

**Método:** GET

**Autenticación:** Requerida

**Parámetros de Ruta:**
| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| id | Guid | ID del tipo de institución |

**Request Example:**
```javascript
const response = await fetch(`http://localhost:7071/api/institution-type/${id}`, {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${token}`
  }
});
```

**Response Success (200 OK):**
```json
{
  "wasSuccessful": true,
  "message": "Tipo de institución obtenido correctamente.",
  "data": {
    "id": "guid",
    "name": "Universidad",
    "isActive": true
  },
  "errors": null
}
```

**Response Errors:**

**404 Not Found:**
```json
{
  "wasSuccessful": false,
  "message": "El tipo de institución no existe.",
  "resultCode": 404
}
```

---

### 4. Actualizar Tipo de Institución

Actualiza el nombre de un tipo de institución existente.

**Endpoint:** `PUT /api/institution-type/{id}`

**Método:** PUT

**Content-Type:** `application/json`

**Autenticación:** Requerida

**Parámetros de Ruta:**
| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| id | Guid | ID del tipo de institución a actualizar |

**Request Body:**
```json
{
  "name": "string"  // Requerido, max 100 chars
}
```

**Request Example:**
```javascript
const response = await fetch(`http://localhost:7071/api/institution-type/${id}`, {
  method: 'PUT',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    name: 'Universidad Pública'
  })
});
```

**Response Success (200 OK):**
```json
{
  "wasSuccessful": true,
  "message": "Tipo de institución actualizado correctamente.",
  "data": {
    "id": "guid",
    "name": "Universidad Pública"
  },
  "errors": null
}
```

**Response Errors:**

**404 Not Found:**
```json
{
  "wasSuccessful": false,
  "message": "El tipo de institución no existe.",
  "resultCode": 404
}
```

**409 Conflict** - Nombre duplicado:
```json
{
  "wasSuccessful": false,
  "message": "Ya existe un tipo de institución con ese nombre.",
  "resultCode": 409
}
```

---

### 5. Eliminar Tipo de Institución

Realiza una eliminación lógica (soft delete) del tipo de institución.

**Endpoint:** `DELETE /api/institution-type/{id}`

**Método:** DELETE

**Autenticación:** Requerida

**Parámetros de Ruta:**
| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| id | Guid | ID del tipo de institución a eliminar |

**Request Example:**
```javascript
const response = await fetch(`http://localhost:7071/api/institution-type/${id}`, {
  method: 'DELETE',
  headers: {
    'Authorization': `Bearer ${token}`
  }
});
```

**Response Success (200 OK):**
```json
{
  "wasSuccessful": true,
  "message": "Tipo de institución eliminado correctamente.",
  "data": true,
  "errors": null
}
```

**Response Errors:**

**404 Not Found:**
```json
{
  "wasSuccessful": false,
  "message": "El tipo de institución no existe.",
  "resultCode": 404
}
```

**Notas:**
- ✅ El tipo de institución no se elimina físicamente, solo se marca como inactivo (IsActive = false)
- ⚠️ Si hay instituciones asociadas a este tipo, considerar manejar la relación apropiadamente

---

## Institutions - Endpoints GET, PUT, DELETE

### 4. Listar Todas las Instituciones

Obtiene todas las instituciones registradas en el sistema con el nombre de su tipo.

**Endpoint:** `GET /api/institution`

**Método:** GET

**Autenticación:** Requerida

**Request Example:**
```javascript
const response = await fetch('http://localhost:7071/api/institution', {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${token}`
  }
});
```

**Response Success (200 OK):**
```json
{
  "wasSuccessful": true,
  "message": "Instituciones obtenidas correctamente.",
  "data": [
    {
      "id": "guid",
      "name": "Universidad Nacional de Ingeniería",
      "institutionTypeId": "guid",
      "institutionTypeName": "Universidad",
      "isActive": true
    },
    {
      "id": "guid",
      "name": "Universidad Católica Boliviana",
      "institutionTypeId": "guid",
      "institutionTypeName": "Universidad",
      "isActive": true
    },
    {
      "id": "guid",
      "name": "Google Bolivia",
      "institutionTypeId": "guid",
      "institutionTypeName": "Empresa",
      "isActive": true
    }
  ],
  "errors": null
}
```

---

### 5. Obtener Institución por ID

Obtiene una institución específica por su ID con el nombre de su tipo.

**Endpoint:** `GET /api/institution/{id}`

**Método:** GET

**Autenticación:** Requerida

**Parámetros de Ruta:**
| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| id | Guid | ID de la institución |

**Request Example:**
```javascript
const response = await fetch(`http://localhost:7071/api/institution/${id}`, {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${token}`
  }
});
```

**Response Success (200 OK):**
```json
{
  "wasSuccessful": true,
  "message": "Institución obtenida correctamente.",
  "data": {
    "id": "guid",
    "name": "Universidad Nacional de Ingeniería",
    "institutionTypeId": "guid",
    "institutionTypeName": "Universidad",
    "isActive": true
  },
  "errors": null
}
```

**Response Errors:**

**404 Not Found:**
```json
{
  "wasSuccessful": false,
  "message": "La institución no existe.",
  "resultCode": 404
}
```

---

### 6. Actualizar Institución

Actualiza el nombre y/o tipo de una institución existente.

**Endpoint:** `PUT /api/institution/{id}`

**Método:** PUT

**Content-Type:** `application/json`

**Autenticación:** Requerida

**Parámetros de Ruta:**
| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| id | Guid | ID de la institución a actualizar |

**Request Body:**
```json
{
  "name": "string",             // Requerido, max 100 chars
  "institutionTypeId": "guid"   // Requerido
}
```

**Request Example:**
```javascript
const response = await fetch(`http://localhost:7071/api/institution/${id}`, {
  method: 'PUT',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    name: 'Universidad Privada Domingo Savio - UPDS',
    institutionTypeId: institutionTypeId
  })
});
```

**Response Success (200 OK):**
```json
{
  "wasSuccessful": true,
  "message": "Institución actualizada correctamente.",
  "data": {
    "id": "guid",
    "name": "Universidad Privada Domingo Savio - UPDS",
    "institutionTypeName": "Universidad"
  },
  "errors": null
}
```

**Response Errors:**

**404 Not Found** - Institución no existe:
```json
{
  "wasSuccessful": false,
  "message": "La institución no existe.",
  "resultCode": 404
}
```

**404 Not Found** - Tipo de institución no existe:
```json
{
  "wasSuccessful": false,
  "message": "El tipo de institución no existe.",
  "resultCode": 404
}
```

**409 Conflict** - Nombre duplicado:
```json
{
  "wasSuccessful": false,
  "message": "Ya existe una institución con ese nombre.",
  "resultCode": 409
}
```

**Notas:**
- ✅ Valida que el tipo de institución nuevo exista antes de actualizar
- ✅ La validación de nombres duplicados es case-insensitive

---

### 7. Eliminar Institución

Realiza una eliminación lógica (soft delete) de la institución.

**Endpoint:** `DELETE /api/institution/{id}`

**Método:** DELETE

**Autenticación:** Requerida

**Parámetros de Ruta:**
| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| id | Guid | ID de la institución a eliminar |

**Request Example:**
```javascript
const response = await fetch(`http://localhost:7071/api/institution/${id}`, {
  method: 'DELETE',
  headers: {
    'Authorization': `Bearer ${token}`
  }
});
```

**Response Success (200 OK):**
```json
{
  "wasSuccessful": true,
  "message": "Institución eliminada correctamente.",
  "data": true,
  "errors": null
}
```

**Response Errors:**

**404 Not Found:**
```json
{
  "wasSuccessful": false,
  "message": "La institución no existe.",
  "resultCode": 404
}
```

**Notas:**
- ✅ La institución no se elimina físicamente, solo se marca como inactiva (IsActive = false)
- ⚠️ Si hay educaciones asociadas a esta institución, considerar el impacto en los registros de miembros
