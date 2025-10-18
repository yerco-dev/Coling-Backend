using Coling.Application.DTOs.AcademicManagement;
using Coling.Application.UseCases.AcademicManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.AcademicManagement;

public class UpdateDegreeEducationFunction
{
    private readonly ILogger<UpdateDegreeEducationFunction> _logger;
    private readonly UpdateDegreeEducationUseCase _updateDegreeEducationUseCase;

    public UpdateDegreeEducationFunction(
        ILogger<UpdateDegreeEducationFunction> logger,
        UpdateDegreeEducationUseCase updateDegreeEducationUseCase)
    {
        _logger = logger;
        _updateDegreeEducationUseCase = updateDegreeEducationUseCase;
    }

    [Function("UpdateDegreeEducation")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "academic/degree-education/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation("Procesando solicitud de actualización de grado académico.");

            // Extraer MemberId del token JWT
            var memberIdClaim = req.HttpContext.User.FindFirst("MemberId");
            if (memberIdClaim == null || !Guid.TryParse(memberIdClaim.Value, out var memberId))
            {
                return new UnauthorizedObjectResult(new
                {
                    wasSuccessful = false,
                    message = "Usuario no autorizado. Solo los miembros pueden editar educación.",
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

            var dto = new UpdateDegreeEducationDto
            {
                InstitutionId = Guid.TryParse(form["institutionId"], out var institutionId) ? institutionId : Guid.Empty,
                Name = form["name"].ToString(),
                Description = form["description"].ToString(),
                AcademicDegree = form["academicDegree"].ToString(),
                Major = form["major"].ToString(),
                Specialization = form["specialization"].ToString(),
                ThesisTitle = form["thesisTitle"].ToString(),
                GPA = decimal.TryParse(form["gpa"], out var gpa) ? gpa : null,
                HasHonors = bool.TryParse(form["hasHonors"], out var hasHonors) && hasHonors,
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

            var result = await _updateDegreeEducationUseCase.ExecuteAsync(
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
            _logger.LogError(ex, "Error inesperado al actualizar el grado académico.");
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
