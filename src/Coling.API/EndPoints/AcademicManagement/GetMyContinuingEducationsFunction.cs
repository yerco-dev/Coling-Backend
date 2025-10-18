using Coling.Application.UseCases.AcademicManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.AcademicManagement;

public class GetMyContinuingEducationsFunction
{
    private readonly ILogger<GetMyContinuingEducationsFunction> _logger;
    private readonly GetMyContinuingEducationsUseCase _getMyContinuingEducationsUseCase;

    public GetMyContinuingEducationsFunction(
        ILogger<GetMyContinuingEducationsFunction> logger,
        GetMyContinuingEducationsUseCase getMyContinuingEducationsUseCase)
    {
        _logger = logger;
        _getMyContinuingEducationsUseCase = getMyContinuingEducationsUseCase;
    }

    [Function("GetMyContinuingEducations")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "academic/continuing-education")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Obteniendo educaciones continuas del miembro.");

            // Extraer MemberId del token JWT
            var memberIdClaim = req.HttpContext.User.FindFirst("MemberId");
            if (memberIdClaim == null || !Guid.TryParse(memberIdClaim.Value, out var memberId))
            {
                return new UnauthorizedObjectResult(new
                {
                    wasSuccessful = false,
                    message = "Usuario no autorizado. Solo los miembros pueden ver sus educaciones continuas.",
                    resultCode = (int)ResultCode.Unauthorized
                });
            }

            var result = await _getMyContinuingEducationsUseCase.ExecuteAsync(memberId);

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
            _logger.LogError(ex, "Error inesperado al obtener educaciones continuas.");
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
