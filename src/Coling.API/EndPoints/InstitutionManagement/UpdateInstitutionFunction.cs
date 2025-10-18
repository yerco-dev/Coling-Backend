using Coling.Application.DTOs.InstitutionManagement;
using Coling.Application.UseCases.InstitutionManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Coling.API.EndPoints.InstitutionManagement;

public class UpdateInstitutionFunction
{
    private readonly ILogger<UpdateInstitutionFunction> _logger;
    private readonly UpdateInstitutionUseCase _updateInstitutionUseCase;

    public UpdateInstitutionFunction(
        ILogger<UpdateInstitutionFunction> logger,
        UpdateInstitutionUseCase updateInstitutionUseCase)
    {
        _logger = logger;
        _updateInstitutionUseCase = updateInstitutionUseCase;
    }

    [Function("UpdateInstitution")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "institution/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation("Procesando solicitud de actualización de institución.");

            if (!Guid.TryParse(id, out var institutionId))
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "ID inválido.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var dto = await JsonSerializer.DeserializeAsync<UpdateInstitutionDto>(req.Body);

            if (dto == null)
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "El cuerpo de la solicitud es inválido.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var result = await _updateInstitutionUseCase.ExecuteAsync(institutionId, dto);

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar la institución.");
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
