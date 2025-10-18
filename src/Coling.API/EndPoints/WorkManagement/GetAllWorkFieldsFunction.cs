using Coling.Application.UseCases.WorkManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.WorkManagement;

public class GetAllWorkFieldsFunction
{
    private readonly ILogger<GetAllWorkFieldsFunction> _logger;
    private readonly GetAllWorkFieldsUseCase _useCase;

    public GetAllWorkFieldsFunction(
        ILogger<GetAllWorkFieldsFunction> logger,
        GetAllWorkFieldsUseCase useCase)
    {
        _logger = logger;
        _useCase = useCase;
    }

    [Function("GetAllWorkFields")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "work-field")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los campos de trabajo.");

            var result = await _useCase.ExecuteAsync();

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
            _logger.LogError(ex, "Error inesperado al obtener los campos de trabajo.");
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
