using Coling.Application.DTOs.AcademicManagement;
using Coling.Application.UseCases.AcademicManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.AcademicManagement;

public class UpdateProfessionalCertificationFunction
{
    private readonly ILogger<UpdateProfessionalCertificationFunction> _logger;
    private readonly UpdateProfessionalCertificationUseCase _updateProfessionalCertificationUseCase;

    public UpdateProfessionalCertificationFunction(
        ILogger<UpdateProfessionalCertificationFunction> logger,
        UpdateProfessionalCertificationUseCase updateProfessionalCertificationUseCase)
    {
        _logger = logger;
        _updateProfessionalCertificationUseCase = updateProfessionalCertificationUseCase;
    }

    [Function("UpdateProfessionalCertification")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "academic/professional-certification/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation("Procesando solicitud de actualización de certificación profesional.");

            // Extraer MemberId del token JWT
            var memberIdClaim = req.HttpContext.User.FindFirst("MemberId");
            if (memberIdClaim == null || !Guid.TryParse(memberIdClaim.Value, out var memberId))
            {
                return new UnauthorizedObjectResult(new
                {
                    wasSuccessful = false,
                    message = "Usuario no autorizado. Solo los miembros pueden editar certificaciones profesionales.",
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

            if (!req.HasFormContentType)
            {
                return new BadRequestObjectResult(new
                {
                    wasSuccessful = false,
                    message = "El request debe ser multipart/form-data.",
                    resultCode = (int)ResultCode.InputError
                });
            }

            var form = await req.ReadFormAsync();

            var dto = new UpdateProfessionalCertificationDto
            {
                InstitutionId = Guid.TryParse(form["institutionId"], out var institutionId) ? institutionId : Guid.Empty,
                Name = form["name"].ToString(),
                Description = form["description"].ToString(),
                CertificationNumber = form["certificationNumber"].ToString(),
                ExpirationDate = DateTime.TryParse(form["expirationDate"], out var expirationDate) ? expirationDate : null,
                RequiresRenewal = bool.TryParse(form["requiresRenewal"], out var requiresRenewal) && requiresRenewal,
                TitleReceived = form["titleReceived"].ToString(),
                StartYear = int.TryParse(form["startYear"], out var startYear) ? startYear : null,
                StartMonth = int.TryParse(form["startMonth"], out var startMonth) ? startMonth : null,
                StartDay = int.TryParse(form["startDay"], out var startDay) ? startDay : null,
                EndYear = int.TryParse(form["endYear"], out var endYear) ? endYear : null,
                EndMonth = int.TryParse(form["endMonth"], out var endMonth) ? endMonth : null,
                EndDay = int.TryParse(form["endDay"], out var endDay) ? endDay : null,
                Status = form["status"].ToString()
            };

            // Obtener el archivo opcional
            var file = form.Files.GetFile("document");
            Stream? fileStream = null;
            string? fileName = null;
            string? contentType = null;

            if (file != null && file.Length > 0)
            {
                fileStream = file.OpenReadStream();
                fileName = file.FileName;
                contentType = file.ContentType;
            }

            var result = await _updateProfessionalCertificationUseCase.ExecuteAsync(
                memberId,
                memberEducationId,
                dto,
                fileStream,
                fileName,
                contentType);

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
            _logger.LogError(ex, "Error inesperado al actualizar la certificación profesional.");
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
