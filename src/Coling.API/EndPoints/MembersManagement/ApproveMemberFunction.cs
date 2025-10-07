using Coling.Aplication.DTOs.MembersManagement;
using Coling.Application.UseCases.MembersManagement;
using Coling.Domain.Entities.ActionResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Coling.API.EndPoints.MembersManagement;

public class ApproveMemberFunction
{
    private readonly ILogger<ApproveMemberFunction> _logger;
    private readonly ApproveMemberUseCase _approveMemberUseCase;
    private readonly IAuthorizationService _authorizationService;

    public ApproveMemberFunction(
        ILogger<ApproveMemberFunction> logger,
        ApproveMemberUseCase approveMemberUseCase,
        IAuthorizationService authorizationService)
    {
        _logger = logger;
        _approveMemberUseCase = approveMemberUseCase;
        _authorizationService = authorizationService;
    }

    [Function("ApproveMember")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "members/approve")] HttpRequest req,
        FunctionContext context)
    {
        try
        {
            _logger.LogInformation("Procesando solicitud de aprobación de miembro.");

            // Verificar autorización
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

            string requestBody;
            using (var reader = new StreamReader(req.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "El cuerpo de la solicitud está vacío.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var dto = JsonSerializer.Deserialize<ApproveMemberDto>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (dto == null)
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "Datos inválidos.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var result = await _approveMemberUseCase.ExecuteAsync(dto);

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
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error al deserializar el request.");
            return new BadRequestObjectResult(new
            {
                wasSuccessful = false,
                message = "Formato JSON inválido.",
                resultCode = (int)ResultCode.InputError
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al aprobar miembro.");
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
