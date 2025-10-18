using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Aplication.Validators;
using Coling.Application.DTOs.WorkManagement;
using Coling.Application.Interfaces.Services.Storage;
using Coling.Application.Mappers.ActionResponse;
using Coling.Application.Mappers.WorkManagement;
using Coling.Application.Validators;
using Coling.Domain.Entities.WorkManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Interfaces.Repositories.MembersManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class RegisterWorkExperienceUseCase
{
    private readonly IWorkExperienceRepository _workExperienceRepository;
    private readonly IWorkExperienceFieldRepository _workExperienceFieldRepository;
    private readonly IWorkFieldRepository _workFieldRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IInstitutionRepository _institutionRepository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IDbContextUnitOfWork _unitOfWork;

    public RegisterWorkExperienceUseCase(
        IWorkExperienceRepository workExperienceRepository,
        IWorkExperienceFieldRepository workExperienceFieldRepository,
        IWorkFieldRepository workFieldRepository,
        IMemberRepository memberRepository,
        IInstitutionRepository institutionRepository,
        IBlobStorageService blobStorageService,
        IDbContextUnitOfWork unitOfWork)
    {
        _workExperienceRepository = workExperienceRepository;
        _workExperienceFieldRepository = workExperienceFieldRepository;
        _workFieldRepository = workFieldRepository;
        _memberRepository = memberRepository;
        _institutionRepository = institutionRepository;
        _blobStorageService = blobStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ActionResponse<WorkExperienceDetailDto>> ExecuteAsync(
        Guid memberId,
        RegisterWorkExperienceDto dto,
        Stream? fileStream = null,
        string? fileName = null,
        string? contentType = null)
    {
        var dtoValidationResult = dto.TryValidateModel();
        if (!dtoValidationResult.WasSuccessful)
            return dtoValidationResult.ChangeNullActionResponseType<RegisterWorkExperienceDto, WorkExperienceDetailDto>();

        var memberValidation = await memberId.ValidateMemberExists(_memberRepository);
        if (!memberValidation.WasSuccessful)
            return memberValidation.ChangeNullActionResponseType<Guid, WorkExperienceDetailDto>();

        var institutionValidation = await dto.InstitutionId.ValidateInstitutionExists(_institutionRepository);
        if (!institutionValidation.WasSuccessful)
            return institutionValidation.ChangeNullActionResponseType<Guid, WorkExperienceDetailDto>();

        var workFieldsValidation = await dto.WorkFieldIds.ValidateWorkFieldsExist(_workFieldRepository);
        if (!workFieldsValidation.WasSuccessful)
            return workFieldsValidation.ChangeNullActionResponseType<List<Guid>, WorkExperienceDetailDto>();

        var workExperience = dto.ToEntity(memberId);
        var modelValidation = workExperience.TryValidateModel();
        if (!modelValidation.WasSuccessful)
            return modelValidation.ChangeNullActionResponseType<WorkExperience, WorkExperienceDetailDto>();

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            string? documentUrl = null;
            if (fileStream != null && !string.IsNullOrEmpty(fileName))
            {
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

                documentUrl = uploadResult.Result;
            }

            workExperience.DocumentUrl = documentUrl;
            var createResult = await _workExperienceRepository.AddAsync(workExperience);

            if (!createResult.WasSuccessful)
            {
                await _unitOfWork.RollbackAsync();
                return createResult.ChangeNullActionResponseType<WorkExperience, WorkExperienceDetailDto>();
            }

            // Crear las relaciones WorkExperienceField
            foreach (var workFieldId in dto.WorkFieldIds)
            {
                var workExperienceField = new WorkExperienceField
                {
                    WorkExperienceId = createResult.Result!.Id,
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

            // Obtener la experiencia con todos los detalles
            var detailedExperience = await _workExperienceRepository.GetByMemberIdWithDetailsAsync(memberId);
            var createdExperience = detailedExperience.Result?.FirstOrDefault(we => we.Id == createResult.Result!.Id);

            if (createdExperience != null)
            {
                return ActionResponse<WorkExperienceDetailDto>.Success(
                    createdExperience.ToDetailDto(),
                    "Experiencia laboral registrada correctamente.");
            }

            return ActionResponse<WorkExperienceDetailDto>.Success(
                createResult.Result!.ToDetailDto(),
                "Experiencia laboral registrada correctamente.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            return ActionResponse<WorkExperienceDetailDto>.Failure(
                $"Error inesperado al registrar la experiencia laboral: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
