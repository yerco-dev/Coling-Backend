using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Coling.Application.Interfaces.Services.Storage;
using Coling.Domain.Wrappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Coling.Infrastructure.Services.Storage;

public class BlobStorageService : IBlobStorageService
{
    private readonly Lazy<BlobServiceClient> _blobServiceClient;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
    {
        _logger = logger;
        _blobServiceClient = new Lazy<BlobServiceClient>(() =>
        {
            // En Azure Functions, las configuraciones de Values se leen directamente
            var connectionString = configuration["AzureBlobStorage"]
                                ?? configuration["Values:AzureBlobStorage"];

            _logger.LogInformation($"Intentando conectar con connection string: {connectionString?.Substring(0, Math.Min(50, connectionString?.Length ?? 0))}...");

            if (string.IsNullOrEmpty(connectionString))
            {
                var errorMsg = "La configuración 'AzureBlobStorage' no está definida en local.settings.json";
                _logger.LogError(errorMsg);
                throw new InvalidOperationException(errorMsg);
            }

            try
            {
                return new BlobServiceClient(connectionString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al crear BlobServiceClient. Connection string: {connectionString}");
                throw;
            }
        });
    }

    public async Task<ActionResponse<string>> UploadFileAsync(string containerName, string fileName, Stream stream, string contentType)
    {
        try
        {
            var containerClient = _blobServiceClient.Value.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(fileName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders
            });

            return ActionResponse<string>.Success(blobClient.Uri.ToString(), "Archivo subido exitosamente");
        }
        catch (Exception ex)
        {
            return ActionResponse<string>.Failure($"Error al subir archivo: {ex.Message}", ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<Stream>> DownloadFileAsync(string containerName, string fileName)
    {
        try
        {
            var containerClient = _blobServiceClient.Value.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync())
            {
                return ActionResponse<Stream>.NotFound("Archivo no encontrado");
            }

            var downloadResponse = await blobClient.DownloadAsync();
            var memoryStream = new MemoryStream();
            await downloadResponse.Value.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            return ActionResponse<Stream>.Success(memoryStream, "Archivo descargado exitosamente");
        }
        catch (Exception ex)
        {
            return ActionResponse<Stream>.Failure($"Error al descargar archivo: {ex.Message}", ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<bool>> DeleteFileAsync(string containerName, string fileName)
    {
        try
        {
            var containerClient = _blobServiceClient.Value.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var result = await blobClient.DeleteIfExistsAsync();

            if (!result.Value)
            {
                return ActionResponse<bool>.NotFound("Archivo no encontrado");
            }

            return ActionResponse<bool>.Success(true, "Archivo eliminado exitosamente");
        }
        catch (Exception ex)
        {
            return ActionResponse<bool>.Failure($"Error al eliminar archivo: {ex.Message}", ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<bool>> FileExistsAsync(string containerName, string fileName)
    {
        try
        {
            var containerClient = _blobServiceClient.Value.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var exists = await blobClient.ExistsAsync();
            return ActionResponse<bool>.Success(exists.Value);
        }
        catch (Exception ex)
        {
            return ActionResponse<bool>.Failure($"Error al verificar archivo: {ex.Message}", ResultCode.DatabaseError);
        }
    }

    public async Task<ActionResponse<string>> GetFileUrlAsync(string containerName, string fileName)
    {
        try
        {
            var containerClient = _blobServiceClient.Value.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync())
            {
                return ActionResponse<string>.NotFound("Archivo no encontrado");
            }

            return ActionResponse<string>.Success(blobClient.Uri.ToString());
        }
        catch (Exception ex)
        {
            return ActionResponse<string>.Failure($"Error al obtener URL: {ex.Message}", ResultCode.DatabaseError);
        }
    }
}
