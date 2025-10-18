using Coling.Application.UseCases.WorkManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.WorkManagement;

public class DeleteWorkExperienceFunction
{
    private readonly ILogger<DeleteWorkExperienceFunction> _logger;
    private readonly DeleteWorkExperienceUseCase _useCase;

    public DeleteWorkExperienceFunction(
        ILogger<DeleteWorkExperienceFunction> logger,
        DeleteWorkExperienceUseCase useCase)
    {
        _logger = logger;
        _useCase = useCase;
    }

    [Function("DeleteWorkExperience")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "work-experience/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation($"Eliminando experiencia laboral con ID: {id}");

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

            if (!Guid.TryParse(id, out var experienceId))
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "ID de experiencia laboral inválido.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var result = await _useCase.ExecuteAsync(experienceId, memberId);

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
            _logger.LogError(ex, "Error inesperado al eliminar experiencia laboral.");
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
