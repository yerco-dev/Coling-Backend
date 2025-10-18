using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Aplication.Validators;
using Coling.Application.DTOs.WorkManagement;
using Coling.Application.Interfaces.Services.Storage;
using Coling.Application.Mappers.ActionResponse;
using Coling.Application.Mappers.WorkManagement;
using Coling.Application.Validators;
using Coling.Domain.Entities.WorkManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class UpdateWorkExperienceUseCase
{
    private readonly IWorkExperienceRepository _workExperienceRepository;
    private readonly IWorkExperienceFieldRepository _workExperienceFieldRepository;
    private readonly IWorkFieldRepository _workFieldRepository;
    private readonly IInstitutionRepository _institutionRepository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IDbContextUnitOfWork _unitOfWork;

    public UpdateWorkExperienceUseCase(
        IWorkExperienceRepository workExperienceRepository,
        IWorkExperienceFieldRepository workExperienceFieldRepository,
        IWorkFieldRepository workFieldRepository,
        IInstitutionRepository institutionRepository,
        IBlobStorageService blobStorageService,
        IDbContextUnitOfWork unitOfWork)
    {
        _workExperienceRepository = workExperienceRepository;
        _workExperienceFieldRepository = workExperienceFieldRepository;
        _workFieldRepository = workFieldRepository;
        _institutionRepository = institutionRepository;
        _blobStorageService = blobStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ActionResponse<WorkExperienceDetailDto>> ExecuteAsync(
        Guid memberId,
        UpdateWorkExperienceDto dto,
        Stream? fileStream = null,
        string? fileName = null,
        string? contentType = null)
    {
        var dtoValidationResult = dto.TryValidateModel();
        if (!dtoValidationResult.WasSuccessful)
            return dtoValidationResult.ChangeNullActionResponseType<UpdateWorkExperienceDto, WorkExperienceDetailDto>();

        var workExperienceResult = await _workExperienceRepository.GetAsync(dto.Id);
        if (!workExperienceResult.WasSuccessful)
            return ActionResponse<WorkExperienceDetailDto>.NotFound("Experiencia laboral no encontrada.");

        var workExperience = workExperienceResult.Result!;

        // Validar ownership
        if (workExperience.MemberId != memberId)
            return ActionResponse<WorkExperienceDetailDto>.Failure("No tienes permiso para modificar esta experiencia laboral.");

        var institutionValidation = await dto.InstitutionId.ValidateInstitutionExists(_institutionRepository);
        if (!institutionValidation.WasSuccessful)
            return institutionValidation.ChangeNullActionResponseType<Guid, WorkExperienceDetailDto>();

        var workFieldsValidation = await dto.WorkFieldIds.ValidateWorkFieldsExist(_workFieldRepository);
        if (!workFieldsValidation.WasSuccessful)
            return workFieldsValidation.ChangeNullActionResponseType<List<Guid>, WorkExperienceDetailDto>();

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Actualizar archivo si se proporciona uno nuevo
            if (fileStream != null && !string.IsNullOrEmpty(fileName))
            {
                // Eliminar el archivo anterior si existe
                if (!string.IsNullOrEmpty(workExperience.DocumentUrl))
                {
                    await _blobStorageService.DeleteFileAsync("work-experience-documents", workExperience.DocumentUrl);
                }

                var fileExtension = Path.GetExtension(fileName);
                var uniqueFileName = $"{memberId}_{Guid.NewGuid()}{fileExtension}";

                var uploadResult = await _blobStorageService.UploadFileAsync(
                    "work-experience-documents",
                    uniqueFileName,
                    fileStream,
                    contentType ?? "application/octet-stream");

                if (!uploadResult.WasSuccessful)
                {
                    await _unitOfWork.RollbackAsync();
                    return uploadResult.ChangeNullActionResponseType<string, WorkExperienceDetailDto>();
                }

                workExperience.DocumentUrl = uploadResult.Result;
            }

            // Actualizar la experiencia laboral
            workExperience.UpdateFromDto(dto);

            var updateResult = await _workExperienceRepository.UpdateAsync(workExperience);
            if (!updateResult.WasSuccessful)
            {
                await _unitOfWork.RollbackAsync();
                return updateResult.ChangeNullActionResponseType<WorkExperience, WorkExperienceDetailDto>();
            }

            // Eliminar las relaciones existentes de work fields (soft delete)
            var existingFields = await _workExperienceFieldRepository.GetByWorkExperienceIdAsync(dto.Id);
            if (existingFields.WasSuccessful)
            {
                foreach (var field in existingFields.Result!)
                {
                    field.IsActive = false;
                    await _workExperienceFieldRepository.UpdateAsync(field);
                }
            }

            // Crear las nuevas relaciones WorkExperienceField
            foreach (var workFieldId in dto.WorkFieldIds)
            {
                var workExperienceField = new WorkExperienceField
                {
                    WorkExperienceId = dto.Id,
                    WorkFieldId = workFieldId
                };

                var createFieldResult = await _workExperienceFieldRepository.AddAsync(workExperienceField);
                if (!createFieldResult.WasSuccessful)
                {
                    await _unitOfWork.RollbackAsync();
                    return createFieldResult.ChangeNullActionResponseType<WorkExperienceField, WorkExperienceDetailDto>();
                }
            }

            await _unitOfWork.CommitAsync();

            // Obtener la experiencia actualizada con todos los detalles
            var detailedExperience = await _workExperienceRepository.GetByMemberIdWithDetailsAsync(memberId);
            var updatedExperience = detailedExperience.Result?.FirstOrDefault(we => we.Id == dto.Id);

            if (updatedExperience != null)
            {
                return ActionResponse<WorkExperienceDetailDto>.Success(
                    updatedExperience.ToDetailDto(),
                    "Experiencia laboral actualizada correctamente.");
            }

            return ActionResponse<WorkExperienceDetailDto>.Success(
                workExperience.ToDetailDto(),
                "Experiencia laboral actualizada correctamente.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            return ActionResponse<WorkExperienceDetailDto>.Failure(
                $"Error inesperado al actualizar la experiencia laboral: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
