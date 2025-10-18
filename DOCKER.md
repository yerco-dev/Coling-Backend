# Docker - Azurite (Emulador de Azure Storage)

## Iniciar Azurite

```bash
docker-compose up -d
```

## Detener Azurite

```bash
docker-compose down
```

## Ver logs

```bash
docker-compose logs -f azurite
```

## Connection String

La connection string ya está configurada en `local.settings.json` en la sección `Values`:

```json
"AzureBlobStorage": "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02qiYm5zVJo+vzVBCR4rQ6eFpNmHJ4NvXTdNdVDHbCqz9M7KhPFRfYbYb4lzR7D4h5ZLBq3O8K5mW==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;"
```

Esta es la AccountKey estándar de Azurite (emulador local de Azure Storage).

Para Azure (producción), reemplaza con tu connection string real de Azure Storage.

## Puertos

- **10000** - Blob Storage
- **10001** - Queue Storage
- **10002** - Table Storage

## Herramientas para visualizar

- **Azure Storage Explorer**: https://azure.microsoft.com/es-es/products/storage/storage-explorer
- **Microsoft Azure Storage Explorer** (Standalone)

### Configurar Azure Storage Explorer

1. Abrir Azure Storage Explorer
2. Conectar a "Local Storage Emulator"
3. Usar la connection string de arriba
