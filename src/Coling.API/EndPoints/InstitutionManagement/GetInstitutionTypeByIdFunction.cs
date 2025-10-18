using Coling.Application.UseCases.InstitutionManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.InstitutionManagement;

public class GetInstitutionTypeByIdFunction
{
    private readonly ILogger<GetInstitutionTypeByIdFunction> _logger;
    private readonly GetInstitutionTypeByIdUseCase _getInstitutionTypeByIdUseCase;

    public GetInstitutionTypeByIdFunction(
        ILogger<GetInstitutionTypeByIdFunction> logger,
        GetInstitutionTypeByIdUseCase getInstitutionTypeByIdUseCase)
    {
        _logger = logger;
        _getInstitutionTypeByIdUseCase = getInstitutionTypeByIdUseCase;
    }

    [Function("GetInstitutionTypeById")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "institution-type/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation("Obteniendo tipo de institución por ID.");

            if (!Guid.TryParse(id, out var institutionTypeId))
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "ID inválido.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var result = await _getInstitutionTypeByIdUseCase.ExecuteAsync(institutionTypeId);

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
            _logger.LogError(ex, "Error inesperado al obtener el tipo de institución.");
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
