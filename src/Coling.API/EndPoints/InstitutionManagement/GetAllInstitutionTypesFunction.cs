using Coling.Application.UseCases.InstitutionManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.InstitutionManagement;

public class GetAllInstitutionTypesFunction
{
    private readonly ILogger<GetAllInstitutionTypesFunction> _logger;
    private readonly GetAllInstitutionTypesUseCase _getAllInstitutionTypesUseCase;

    public GetAllInstitutionTypesFunction(
        ILogger<GetAllInstitutionTypesFunction> logger,
        GetAllInstitutionTypesUseCase getAllInstitutionTypesUseCase)
    {
        _logger = logger;
        _getAllInstitutionTypesUseCase = getAllInstitutionTypesUseCase;
    }

    [Function("GetAllInstitutionTypes")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "institution-type")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los tipos de institución.");

            var result = await _getAllInstitutionTypesUseCase.ExecuteAsync();

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
            _logger.LogError(ex, "Error inesperado al obtener los tipos de institución.");
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
