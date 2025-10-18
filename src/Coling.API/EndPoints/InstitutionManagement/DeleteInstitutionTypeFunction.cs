using Coling.Application.UseCases.InstitutionManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.InstitutionManagement;

public class DeleteInstitutionTypeFunction
{
    private readonly ILogger<DeleteInstitutionTypeFunction> _logger;
    private readonly DeleteInstitutionTypeUseCase _deleteInstitutionTypeUseCase;

    public DeleteInstitutionTypeFunction(
        ILogger<DeleteInstitutionTypeFunction> logger,
        DeleteInstitutionTypeUseCase deleteInstitutionTypeUseCase)
    {
        _logger = logger;
        _deleteInstitutionTypeUseCase = deleteInstitutionTypeUseCase;
    }

    [Function("DeleteInstitutionType")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "institution-type/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation("Procesando solicitud de eliminación de tipo de institución.");

            if (!Guid.TryParse(id, out var institutionTypeId))
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "ID inválido.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var result = await _deleteInstitutionTypeUseCase.ExecuteAsync(institutionTypeId);

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
            _logger.LogError(ex, "Error inesperado al eliminar el tipo de institución.");
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
