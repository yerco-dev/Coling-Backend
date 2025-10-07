using Coling.Aplication.DTOs.UsersManagement;
using Coling.Application.UseCases.UsersManagement;
using Coling.Domain.Entities.ActionResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Coling.API.EndPoints.UsersManagement;

public class RegisterMemberFunction
{
    private readonly ILogger<RegisterMemberFunction> _logger;
    private readonly RegisterMemberUserUseCase _registerMemberUserUseCase;

    public RegisterMemberFunction(
        ILogger<RegisterMemberFunction> logger,
        RegisterMemberUserUseCase registerMemberUserUseCase)
    {
        _logger = logger;
        _registerMemberUserUseCase = registerMemberUserUseCase;
    }

    [Function("RegisterMember")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "auth/register-member")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Procesando solicitud de registro de miembro.");

            // Leer el body del request
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

            // Deserializar el DTO
            var dto = JsonSerializer.Deserialize<RegisterMemberUserDto>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (dto == null)
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "Datos de registro inválidos.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var result = await _registerMemberUserUseCase.ExecuteAsync(dto);

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
            _logger.LogError(ex, "Error inesperado al procesar el registro de miembro.");
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
