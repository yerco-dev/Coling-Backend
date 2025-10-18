using Coling.Application.UseCases.WorkManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.WorkManagement;

public class GetWorkFieldCategoryByIdFunction
{
    private readonly ILogger<GetWorkFieldCategoryByIdFunction> _logger;
    private readonly GetWorkFieldCategoryByIdUseCase _useCase;

    public GetWorkFieldCategoryByIdFunction(
        ILogger<GetWorkFieldCategoryByIdFunction> logger,
        GetWorkFieldCategoryByIdUseCase useCase)
    {
        _logger = logger;
        _useCase = useCase;
    }

    [Function("GetWorkFieldCategoryById")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "work-field-category/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation($"Obteniendo categoría de campo de trabajo con ID: {id}");

            if (!Guid.TryParse(id, out var categoryId))
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "ID de categoría inválido.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var result = await _useCase.ExecuteAsync(categoryId);

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
            _logger.LogError(ex, "Error inesperado al obtener la categoría de campo de trabajo.");
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
