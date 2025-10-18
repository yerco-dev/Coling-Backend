using Coling.Application.UseCases.AcademicManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.AcademicManagement;

public class GetMyDegreeEducationsFunction
{
    private readonly ILogger<GetMyDegreeEducationsFunction> _logger;
    private readonly GetMyDegreeEducationsUseCase _getMyDegreeEducationsUseCase;

    public GetMyDegreeEducationsFunction(
        ILogger<GetMyDegreeEducationsFunction> logger,
        GetMyDegreeEducationsUseCase getMyDegreeEducationsUseCase)
    {
        _logger = logger;
        _getMyDegreeEducationsUseCase = getMyDegreeEducationsUseCase;
    }

    [Function("GetMyDegreeEducations")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "academic/degree-education")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Obteniendo grados académicos del miembro.");

            // Extraer MemberId del token JWT
            var memberIdClaim = req.HttpContext.User.FindFirst("MemberId");
            if (memberIdClaim == null || !Guid.TryParse(memberIdClaim.Value, out var memberId))
            {
                return new UnauthorizedObjectResult(new
                {
                    wasSuccessful = false,
                    message = "Usuario no autorizado. Solo los miembros pueden ver sus grados académicos.",
                    resultCode = (int)ResultCode.Unauthorized
                });
            }

            var result = await _getMyDegreeEducationsUseCase.ExecuteAsync(memberId);

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
            _logger.LogError(ex, "Error inesperado al obtener grados académicos.");
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
