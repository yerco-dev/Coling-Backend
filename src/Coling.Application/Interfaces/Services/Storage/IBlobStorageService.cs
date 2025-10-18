using Coling.Domain.Wrappers;

namespace Coling.Application.Interfaces.Services.Storage;

public interface IBlobStorageService
{
    /// <summary>
    /// Sube un archivo al blob storage
    /// </summary>
    /// <param name="containerName">Nombre del contenedor</param>
    /// <param name="fileName">Nombre del archivo</param>
    /// <param name="stream">Stream del archivo</param>
    /// <param name="contentType">Tipo de contenido (ej: image/png)</param>
    /// <returns>URL del archivo subido</returns>
    Task<ActionResponse<string>> UploadFileAsync(string containerName, string fileName, Stream stream, string contentType);

    /// <summary>
    /// Descarga un archivo del blob storage
    /// </summary>
    Task<ActionResponse<Stream>> DownloadFileAsync(string containerName, string fileName);

    /// <summary>
    /// Elimina un archivo del blob storage
    /// </summary>
    Task<ActionResponse<bool>> DeleteFileAsync(string containerName, string fileName);

    /// <summary>
    /// Verifica si un archivo existe
    /// </summary>
    Task<ActionResponse<bool>> FileExistsAsync(string containerName, string fileName);

    /// <summary>
    /// Obtiene la URL de un archivo
    /// </summary>
    Task<ActionResponse<string>> GetFileUrlAsync(string containerName, string fileName);
}
