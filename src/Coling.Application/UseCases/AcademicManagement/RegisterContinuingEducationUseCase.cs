using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Aplication.Validators;
using Coling.Application.DTOs.AcademicManagement;
using Coling.Application.Interfaces.Services.Storage;
using Coling.Application.Mappers.ActionResponse;
using Coling.Application.Mappers.AcademicManagement;
using Coling.Application.Validators;
using Coling.Domain.Interfaces.Repositories.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.AcademicManagement;

public class RegisterContinuingEducationUseCase
{
    private readonly IContinuingEducationRepository _continuingEducationRepository;
    private readonly IMemberEducationRepository _memberEducationRepository;
    private readonly IInstitutionRepository _institutionRepository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IDbContextUnitOfWork _unitOfWork;

    public RegisterContinuingEducationUseCase(
        IContinuingEducationRepository continuingEducationRepository,
        IMemberEducationRepository memberEducationRepository,
        IInstitutionRepository institutionRepository,
        IBlobStorageService blobStorageService,
        IDbContextUnitOfWork unitOfWork)
    {
        _continuingEducationRepository = continuingEducationRepository;
        _memberEducationRepository = memberEducationRepository;
        _institutionRepository = institutionRepository;
        _blobStorageService = blobStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ActionResponse<ContinuingEducationCreatedDto>> ExecuteAsync(
        Guid memberId,
        RegisterContinuingEducationDto dto,
        Stream? fileStream = null,
        string? fileName = null,
        string? contentType = null)
    {
        // Validar DTO
        var dtoValidationResult = dto.TryValidateModel();
        if (!dtoValidationResult.WasSuccessful)
            return dtoValidationResult.ChangeNullActionResponseType<RegisterContinuingEducationDto, ContinuingEducationCreatedDto>();

        // Validar que la institución existe
        var institutionValidation = await dto.InstitutionId.ValidateInstitutionExists(_institutionRepository);
        if (!institutionValidation.WasSuccessful)
            return institutionValidation.ChangeNullActionResponseType<Guid, ContinuingEducationCreatedDto>();

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Crear ContinuingEducation
            var education = dto.ToContinuingEducation();
            var educationResult = await _continuingEducationRepository.AddAsync(education);

            if (!educationResult.WasSuccessful)
            {
                await _unitOfWork.RollbackAsync();
                return educationResult.ChangeNullActionResponseType<Domain.Entities.AcademicManagement.ContinuingEducation, ContinuingEducationCreatedDto>();
            }

            // Crear MemberEducation
            var memberEducation = dto.ToMemberEducation(memberId, education.Id);

            // Subir archivo si existe
            if (fileStream != null && !string.IsNullOrEmpty(fileName))
            {
                var fileExtension = Path.GetExtension(fileName);
                var uniqueFileName = $"{memberId}_{Guid.NewGuid()}{fileExtension}";

                var uploadResult = await _blobStorageService.UploadFileAsync(
                    "academic-documents",
                    uniqueFileName,
                    fileStream,
                    contentType ?? "application/octet-stream");

                if (!uploadResult.WasSuccessful)
                {
                    await _unitOfWork.RollbackAsync();
                    return uploadResult.ChangeNullActionResponseType<string, ContinuingEducationCreatedDto>();
                }

                memberEducation.DocumentUrl = uploadResult.Result;
            }

            var memberEducationResult = await _memberEducationRepository.AddAsync(memberEducation);

            if (!memberEducationResult.WasSuccessful)
            {
                await _unitOfWork.RollbackAsync();
                return memberEducationResult.ChangeNullActionResponseType<Domain.Entities.AcademicManagement.MemberEducation, ContinuingEducationCreatedDto>();
            }

            await _unitOfWork.CommitAsync();

            // Obtener institución para respuesta
            var institution = await _institutionRepository.GetAsync(dto.InstitutionId);

            return ActionResponse<ContinuingEducationCreatedDto>.Success(
                memberEducation.ToContinuingEducationCreatedDto(education, institution.Result?.Name ?? ""),
                "Educación continua registrada correctamente.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ActionResponse<ContinuingEducationCreatedDto>.Failure(
                $"Error al registrar la educación continua: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
