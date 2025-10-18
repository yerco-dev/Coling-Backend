using Coling.Application.DTOs.InstitutionManagement;
using Coling.Application.UseCases.InstitutionManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Coling.API.EndPoints.InstitutionManagement;

public class RegisterInstitutionFunction
{
    private readonly ILogger<RegisterInstitutionFunction> _logger;
    private readonly RegisterInstitutionUseCase _registerInstitutionUseCase;

    public RegisterInstitutionFunction(
        ILogger<RegisterInstitutionFunction> logger,
        RegisterInstitutionUseCase registerInstitutionUseCase)
    {
        _logger = logger;
        _registerInstitutionUseCase = registerInstitutionUseCase;
    }

    [Function("RegisterInstitution")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "institution")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Procesando solicitud de registro de institución.");

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

            var dto = JsonSerializer.Deserialize<RegisterInstitutionDto>(requestBody, new JsonSerializerOptions
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

            var result = await _registerInstitutionUseCase.ExecuteAsync(dto);

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
            _logger.LogError(ex, "Error inesperado al procesar el registro de institución.");
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
