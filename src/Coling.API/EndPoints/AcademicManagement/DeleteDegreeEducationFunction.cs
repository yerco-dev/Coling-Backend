using Coling.Application.UseCases.AcademicManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.AcademicManagement;

public class DeleteDegreeEducationFunction
{
    private readonly ILogger<DeleteDegreeEducationFunction> _logger;
    private readonly DeleteDegreeEducationUseCase _deleteDegreeEducationUseCase;

    public DeleteDegreeEducationFunction(
        ILogger<DeleteDegreeEducationFunction> logger,
        DeleteDegreeEducationUseCase deleteDegreeEducationUseCase)
    {
        _logger = logger;
        _deleteDegreeEducationUseCase = deleteDegreeEducationUseCase;
    }

    [Function("DeleteDegreeEducation")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "academic/degree-education/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation("Procesando solicitud de eliminación de grado académico.");

            // Extraer MemberId del token JWT
            var memberIdClaim = req.HttpContext.User.FindFirst("MemberId");
            if (memberIdClaim == null || !Guid.TryParse(memberIdClaim.Value, out var memberId))
            {
                return new UnauthorizedObjectResult(new
                {
                    wasSuccessful = false,
                    message = "Usuario no autorizado. Solo los miembros pueden eliminar su educación.",
                    resultCode = (int)ResultCode.Unauthorized
                });
            }

            // Validar ID
            if (!Guid.TryParse(id, out var memberEducationId))
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "ID inválido.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var result = await _deleteDegreeEducationUseCase.ExecuteAsync(memberId, memberEducationId);

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
            _logger.LogError(ex, "Error inesperado al eliminar el grado académico.");
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
