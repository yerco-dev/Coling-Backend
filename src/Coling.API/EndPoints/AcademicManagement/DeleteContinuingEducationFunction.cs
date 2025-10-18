using Coling.Application.UseCases.AcademicManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.AcademicManagement;

public class DeleteContinuingEducationFunction
{
    private readonly ILogger<DeleteContinuingEducationFunction> _logger;
    private readonly DeleteContinuingEducationUseCase _deleteContinuingEducationUseCase;

    public DeleteContinuingEducationFunction(
        ILogger<DeleteContinuingEducationFunction> logger,
        DeleteContinuingEducationUseCase deleteContinuingEducationUseCase)
    {
        _logger = logger;
        _deleteContinuingEducationUseCase = deleteContinuingEducationUseCase;
    }

    [Function("DeleteContinuingEducation")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "academic/continuing-education/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation("Procesando solicitud de eliminación de educación continua.");

            // Extraer MemberId del token JWT
            var memberIdClaim = req.HttpContext.User.FindFirst("MemberId");
            if (memberIdClaim == null || !Guid.TryParse(memberIdClaim.Value, out var memberId))
            {
                return new UnauthorizedObjectResult(new
                {
                    wasSuccessful = false,
                    message = "Usuario no autorizado. Solo los miembros pueden eliminar su educación continua.",
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

            var result = await _deleteContinuingEducationUseCase.ExecuteAsync(memberId, memberEducationId);

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
            _logger.LogError(ex, "Error inesperado al eliminar la educación continua.");
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
