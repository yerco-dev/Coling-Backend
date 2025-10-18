using Coling.Application.UseCases.InstitutionManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.InstitutionManagement;

public class GetInstitutionByIdFunction
{
    private readonly ILogger<GetInstitutionByIdFunction> _logger;
    private readonly GetInstitutionByIdUseCase _getInstitutionByIdUseCase;

    public GetInstitutionByIdFunction(
        ILogger<GetInstitutionByIdFunction> logger,
        GetInstitutionByIdUseCase getInstitutionByIdUseCase)
    {
        _logger = logger;
        _getInstitutionByIdUseCase = getInstitutionByIdUseCase;
    }

    [Function("GetInstitutionById")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "institution/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation("Obteniendo institución por ID.");

            if (!Guid.TryParse(id, out var institutionId))
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "ID inválido.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var result = await _getInstitutionByIdUseCase.ExecuteAsync(institutionId);

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
            _logger.LogError(ex, "Error inesperado al obtener la institución.");
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
