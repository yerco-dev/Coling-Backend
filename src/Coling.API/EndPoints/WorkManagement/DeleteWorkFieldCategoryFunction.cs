using Coling.Application.UseCases.WorkManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.WorkManagement;

public class DeleteWorkFieldCategoryFunction
{
    private readonly ILogger<DeleteWorkFieldCategoryFunction> _logger;
    private readonly DeleteWorkFieldCategoryUseCase _useCase;

    public DeleteWorkFieldCategoryFunction(
        ILogger<DeleteWorkFieldCategoryFunction> logger,
        DeleteWorkFieldCategoryUseCase useCase)
    {
        _logger = logger;
        _useCase = useCase;
    }

    [Function("DeleteWorkFieldCategory")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "work-field-category/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation($"Eliminando categoría de campo de trabajo con ID: {id}");

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
            _logger.LogError(ex, "Error inesperado al eliminar la categoría de campo de trabajo.");
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
