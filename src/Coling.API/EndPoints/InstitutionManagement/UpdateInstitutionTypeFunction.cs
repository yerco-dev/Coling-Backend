using Coling.Application.DTOs.InstitutionManagement;
using Coling.Application.UseCases.InstitutionManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Coling.API.EndPoints.InstitutionManagement;

public class UpdateInstitutionTypeFunction
{
    private readonly ILogger<UpdateInstitutionTypeFunction> _logger;
    private readonly UpdateInstitutionTypeUseCase _updateInstitutionTypeUseCase;

    public UpdateInstitutionTypeFunction(
        ILogger<UpdateInstitutionTypeFunction> logger,
        UpdateInstitutionTypeUseCase updateInstitutionTypeUseCase)
    {
        _logger = logger;
        _updateInstitutionTypeUseCase = updateInstitutionTypeUseCase;
    }

    [Function("UpdateInstitutionType")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "institution-type/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation("Procesando solicitud de actualización de tipo de institución.");

            if (!Guid.TryParse(id, out var institutionTypeId))
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "ID inválido.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var dto = await JsonSerializer.DeserializeAsync<UpdateInstitutionTypeDto>(req.Body);

            if (dto == null)
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "El cuerpo de la solicitud es inválido.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var result = await _updateInstitutionTypeUseCase.ExecuteAsync(institutionTypeId, dto);

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
            _logger.LogError(ex, "Error inesperado al actualizar el tipo de institución.");
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
