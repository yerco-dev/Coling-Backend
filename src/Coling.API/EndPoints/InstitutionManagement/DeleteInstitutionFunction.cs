using Coling.Application.UseCases.InstitutionManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.InstitutionManagement;

public class DeleteInstitutionFunction
{
    private readonly ILogger<DeleteInstitutionFunction> _logger;
    private readonly DeleteInstitutionUseCase _deleteInstitutionUseCase;

    public DeleteInstitutionFunction(
        ILogger<DeleteInstitutionFunction> logger,
        DeleteInstitutionUseCase deleteInstitutionUseCase)
    {
        _logger = logger;
        _deleteInstitutionUseCase = deleteInstitutionUseCase;
    }

    [Function("DeleteInstitution")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "institution/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation("Procesando solicitud de eliminación de institución.");

            if (!Guid.TryParse(id, out var institutionId))
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "ID inválido.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var result = await _deleteInstitutionUseCase.ExecuteAsync(institutionId);

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
            _logger.LogError(ex, "Error inesperado al eliminar la institución.");
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
