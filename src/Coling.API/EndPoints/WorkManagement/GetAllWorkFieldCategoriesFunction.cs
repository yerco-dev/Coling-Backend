using Coling.Application.UseCases.WorkManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.WorkManagement;

public class GetAllWorkFieldCategoriesFunction
{
    private readonly ILogger<GetAllWorkFieldCategoriesFunction> _logger;
    private readonly GetAllWorkFieldCategoriesUseCase _useCase;

    public GetAllWorkFieldCategoriesFunction(
        ILogger<GetAllWorkFieldCategoriesFunction> logger,
        GetAllWorkFieldCategoriesUseCase useCase)
    {
        _logger = logger;
        _useCase = useCase;
    }

    [Function("GetAllWorkFieldCategories")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "work-field-category")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Obteniendo todas las categorías de campos de trabajo.");

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
            _logger.LogError(ex, "Error inesperado al obtener las categorías de campos de trabajo.");
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
