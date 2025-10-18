using Coling.Application.DTOs.InstitutionManagement;
using Coling.Application.UseCases.InstitutionManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Coling.API.EndPoints.InstitutionManagement;

public class RegisterInstitutionTypeFunction
{
    private readonly ILogger<RegisterInstitutionTypeFunction> _logger;
    private readonly RegisterInstitutionTypeUseCase _registerInstitutionTypeUseCase;

    public RegisterInstitutionTypeFunction(
        ILogger<RegisterInstitutionTypeFunction> logger,
        RegisterInstitutionTypeUseCase registerInstitutionTypeUseCase)
    {
        _logger = logger;
        _registerInstitutionTypeUseCase = registerInstitutionTypeUseCase;
    }

    [Function("RegisterInstitutionType")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "institution-type")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Procesando solicitud de registro de tipo de institución.");

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

            var dto = JsonSerializer.Deserialize<RegisterInstitutionTypeDto>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (dto == null)
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "Datos de registro inválidos.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var result = await _registerInstitutionTypeUseCase.ExecuteAsync(dto);

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
            _logger.LogError(ex, "Error inesperado al procesar el registro de tipo de institución.");
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
