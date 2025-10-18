using Coling.Application.UseCases.WorkManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.WorkManagement;

public class GetMyWorkExperiencesFunction
{
    private readonly ILogger<GetMyWorkExperiencesFunction> _logger;
    private readonly GetMyWorkExperiencesUseCase _useCase;

    public GetMyWorkExperiencesFunction(
        ILogger<GetMyWorkExperiencesFunction> logger,
        GetMyWorkExperiencesUseCase useCase)
    {
        _logger = logger;
        _useCase = useCase;
    }

    [Function("GetMyWorkExperiences")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "work-experience")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Obteniendo experiencias laborales del miembro.");

            // Extraer MemberId del token JWT
            var memberIdClaim = req.HttpContext.User.FindFirst("MemberId");
            if (memberIdClaim == null || !Guid.TryParse(memberIdClaim.Value, out var memberId))
            {
                return new UnauthorizedObjectResult(new
                {
                    wasSuccessful = false,
                    message = "Usuario no autorizado.",
                    resultCode = (int)ResultCode.Unauthorized
                });
            }

            var result = await _useCase.ExecuteAsync(memberId);

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
            _logger.LogError(ex, "Error inesperado al obtener experiencias laborales.");
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
