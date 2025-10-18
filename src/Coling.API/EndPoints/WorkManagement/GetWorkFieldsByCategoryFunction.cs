using Coling.Application.UseCases.WorkManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.WorkManagement;

public class GetWorkFieldsByCategoryFunction
{
    private readonly ILogger<GetWorkFieldsByCategoryFunction> _logger;
    private readonly GetWorkFieldsByCategoryUseCase _useCase;

    public GetWorkFieldsByCategoryFunction(
        ILogger<GetWorkFieldsByCategoryFunction> logger,
        GetWorkFieldsByCategoryUseCase useCase)
    {
        _logger = logger;
        _useCase = useCase;
    }

    [Function("GetWorkFieldsByCategory")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "work-field/category/{categoryId}")] HttpRequest req,
        string categoryId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo campos de trabajo de la categoría: {categoryId}");

            if (!Guid.TryParse(categoryId, out var parsedCategoryId))
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "ID de categoría inválido.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var result = await _useCase.ExecuteAsync(parsedCategoryId);

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
            _logger.LogError(ex, "Error inesperado al obtener los campos de trabajo por categoría.");
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
