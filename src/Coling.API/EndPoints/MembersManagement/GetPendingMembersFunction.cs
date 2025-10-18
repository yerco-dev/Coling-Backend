using Coling.Application.UseCases.MembersManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.MembersManagement;

public class GetPendingMembersFunction
{
    private readonly ILogger<GetPendingMembersFunction> _logger;
    private readonly GetPendingMembersUseCase _getPendingMembersUseCase;
    private readonly IAuthorizationService _authorizationService;

    public GetPendingMembersFunction(
        ILogger<GetPendingMembersFunction> logger,
        GetPendingMembersUseCase getPendingMembersUseCase,
        IAuthorizationService authorizationService)
    {
        _logger = logger;
        _getPendingMembersUseCase = getPendingMembersUseCase;
        _authorizationService = authorizationService;
    }

    [Function("GetPendingMembers")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "members/pending")] HttpRequest req,
        FunctionContext context)
    {
        try
        {
            _logger.LogInformation("Obteniendo miembros pendientes de aprobación.");

            // Verificar autorización usando la política AdminOrModerator
            var user = context.GetHttpContext()?.User;
            if (user == null)
            {
                return new UnauthorizedObjectResult(new
                {
                    wasSuccessful = false,
                    message = "Se requiere autenticación",
                    resultCode = (int)ResultCode.Unauthorized
                });
            }

            var authResult = await _authorizationService.AuthorizeAsync(user, "AdminOrModerator");
            if (!authResult.Succeeded)
            {
                _logger.LogWarning("Acceso denegado por la política 'AdminOrModerator'.");
                return new ObjectResult(new
                {
                    wasSuccessful = false,
                    message = "No tiene permisos para acceder a este recurso",
                    resultCode = (int)ResultCode.Forbidden
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            var result = await _getPendingMembersUseCase.ExecuteAsync();

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
            _logger.LogError(ex, "Error inesperado al obtener miembros pendientes.");
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
