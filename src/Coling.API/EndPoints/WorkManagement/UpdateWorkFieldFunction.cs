using Coling.Application.DTOs.WorkManagement;
using Coling.Application.UseCases.WorkManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Coling.API.EndPoints.WorkManagement;

public class UpdateWorkFieldFunction
{
    private readonly ILogger<UpdateWorkFieldFunction> _logger;
    private readonly UpdateWorkFieldUseCase _useCase;

    public UpdateWorkFieldFunction(
        ILogger<UpdateWorkFieldFunction> logger,
        UpdateWorkFieldUseCase useCase)
    {
        _logger = logger;
        _useCase = useCase;
    }

    [Function("UpdateWorkField")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "work-field/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation($"Actualizando campo de trabajo con ID: {id}");

            if (!Guid.TryParse(id, out var workFieldId))
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "ID de campo de trabajo inválido.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            string requestBody;
            using (var reader = new StreamReader(req.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "El cuerpo de la solicitud está vacío.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var dto = JsonSerializer.Deserialize<WorkFieldUpdateDto>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (dto == null || dto.Id != workFieldId)
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "Datos de actualización inválidos o el ID no coincide.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var result = await _useCase.ExecuteAsync(dto);

            return new ObjectResult(new
            {
                wasSuccessful = result.WasSuccessful,
                message = result.Message,
                data = result.Result,
                errors = result.Errors
            })
            {
                StatusCode = (int)result.ResultCode
            };
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error al deserializar el request.");
            return new BadRequestObjectResult(new
            {
                wasSuccessful = false,
                message = "Formato JSON inválido.",
                resultCode = (int)ResultCode.InputError
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar el campo de trabajo.");
            return new ObjectResult(new
            {
                wasSuccessful = false,
                message = "Error interno del servidor.",
                resultCode = (int)ResultCode.DatabaseError
            })
            { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}
