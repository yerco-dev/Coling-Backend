using Coling.Application.DTOs.Storage;
using Coling.Application.Interfaces.Services.Storage;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.Storage;

public class UploadFileFunction
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly ILogger<UploadFileFunction> _logger;

    public UploadFileFunction(IBlobStorageService blobStorageService, ILogger<UploadFileFunction> logger)
    {
        _blobStorageService = blobStorageService;
        _logger = logger;
    }

    [Function("UploadFile")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "storage/upload")] HttpRequest req)
    {
        try
        {
            var form = await req.ReadFormAsync();
            var file = form.Files.FirstOrDefault();

            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult(ActionResponse<FileUploadResponseDto>.Failure(
                    "No se proporcionó ningún archivo",
                    ResultCode.InputError));
            }

            if (file.Length > 10 * 1024 * 1024)
            {
                return new BadRequestObjectResult(ActionResponse<FileUploadResponseDto>.Failure(
                    "El archivo no debe superar los 10 MB",
                    ResultCode.InputError));
            }

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var containerName = "test-uploads";

            using var stream = file.OpenReadStream();
            var uploadResult = await _blobStorageService.UploadFileAsync(
                containerName,
                fileName,
                stream,
                file.ContentType);

            if (!uploadResult.WasSuccessful)
            {
                return new ObjectResult(uploadResult)
                {
                    StatusCode = (int)uploadResult.ResultCode
                };
            }

            var response = new FileUploadResponseDto
            {
                FileName = fileName,
                FileUrl = uploadResult.Result!,
                ContentType = file.ContentType,
                FileSize = file.Length
            };

            return new OkObjectResult(ActionResponse<FileUploadResponseDto>.Success(
                response,
                "Archivo subido exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir archivo");
            return new ObjectResult(ActionResponse<FileUploadResponseDto>.Failure(
                $"Error interno del servidor: {ex.Message}",
                ResultCode.DatabaseError))
            {
                StatusCode = 500
            };
        }
    }
}
