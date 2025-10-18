using Coling.Application.UseCases.InstitutionManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.InstitutionManagement;

public class GetAllInstitutionsFunction
{
    private readonly ILogger<GetAllInstitutionsFunction> _logger;
    private readonly GetAllInstitutionsUseCase _getAllInstitutionsUseCase;

    public GetAllInstitutionsFunction(
        ILogger<GetAllInstitutionsFunction> logger,
        GetAllInstitutionsUseCase getAllInstitutionsUseCase)
    {
        _logger = logger;
        _getAllInstitutionsUseCase = getAllInstitutionsUseCase;
    }

    [Function("GetAllInstitutions")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "institution")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Obteniendo todas las instituciones.");

            var result = await _getAllInstitutionsUseCase.ExecuteAsync();

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
            _logger.LogError(ex, "Error inesperado al obtener las instituciones.");
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
