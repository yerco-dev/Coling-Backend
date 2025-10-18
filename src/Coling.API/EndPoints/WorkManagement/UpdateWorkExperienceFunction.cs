using Coling.Application.DTOs.WorkManagement;
using Coling.Application.UseCases.WorkManagement;
using Coling.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Coling.API.EndPoints.WorkManagement;

public class UpdateWorkExperienceFunction
{
    private readonly ILogger<UpdateWorkExperienceFunction> _logger;
    private readonly UpdateWorkExperienceUseCase _useCase;

    public UpdateWorkExperienceFunction(
        ILogger<UpdateWorkExperienceFunction> logger,
        UpdateWorkExperienceUseCase useCase)
    {
        _logger = logger;
        _useCase = useCase;
    }

    [Function("UpdateWorkExperience")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "work-experience/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation($"Actualizando experiencia laboral con ID: {id}");

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

            // Parse workFieldIds
            var workFieldIds = new List<Guid>();
            foreach (var key in form.Keys.Where(k => k.StartsWith("workFieldIds[")))
            {
                if (Guid.TryParse(form[key], out var workFieldId))
                    workFieldIds.Add(workFieldId);
            }

            var dto = new UpdateWorkExperienceDto
            {
                Id = experienceId,
                InstitutionId = Guid.TryParse(form["institutionId"], out var institutionId) ? institutionId : Guid.Empty,
                JobTitle = form["jobTitle"].ToString(),
                StartYear = int.TryParse(form["startYear"], out var startYear) ? startYear : null,
                StartMonth = int.TryParse(form["startMonth"], out var startMonth) ? startMonth : null,
                StartDay = int.TryParse(form["startDay"], out var startDay) ? startDay : null,
                EndYear = int.TryParse(form["endYear"], out var endYear) ? endYear : null,
                EndMonth = int.TryParse(form["endMonth"], out var endMonth) ? endMonth : null,
                EndDay = int.TryParse(form["endDay"], out var endDay) ? endDay : null,
                Description = form["description"].ToString(),
                Responsibilities = form["responsibilities"].ToString(),
                Achievements = form["achievements"].ToString(),
                WorkFieldIds = workFieldIds
            };

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

            var result = await _useCase.ExecuteAsync(memberId, dto, fileStream, fileName, contentType);

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
            _logger.LogError(ex, "Error inesperado al actualizar experiencia laboral.");
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
