using Coling.Application.UseCases.WorkManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.WorkManagement;

public class DeleteWorkFieldFunction
{
    private readonly ILogger<DeleteWorkFieldFunction> _logger;
    private readonly DeleteWorkFieldUseCase _useCase;

    public DeleteWorkFieldFunction(
        ILogger<DeleteWorkFieldFunction> logger,
        DeleteWorkFieldUseCase useCase)
    {
        _logger = logger;
        _useCase = useCase;
    }

    [Function("DeleteWorkField")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "work-field/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation($"Eliminando campo de trabajo con ID: {id}");

            if (!Guid.TryParse(id, out var workFieldId))
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "ID de campo de trabajo inválido.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var result = await _useCase.ExecuteAsync(workFieldId);

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
            _logger.LogError(ex, "Error inesperado al eliminar el campo de trabajo.");
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
