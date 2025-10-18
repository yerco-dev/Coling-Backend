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

public class RegisterProfessionalCertificationUseCase
{
    private readonly IProfessionalCertificationRepository _professionalCertificationRepository;
    private readonly IMemberEducationRepository _memberEducationRepository;
    private readonly IInstitutionRepository _institutionRepository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IDbContextUnitOfWork _unitOfWork;

    public RegisterProfessionalCertificationUseCase(
        IProfessionalCertificationRepository professionalCertificationRepository,
        IMemberEducationRepository memberEducationRepository,
        IInstitutionRepository institutionRepository,
        IBlobStorageService blobStorageService,
        IDbContextUnitOfWork unitOfWork)
    {
        _professionalCertificationRepository = professionalCertificationRepository;
        _memberEducationRepository = memberEducationRepository;
        _institutionRepository = institutionRepository;
        _blobStorageService = blobStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ActionResponse<ProfessionalCertificationCreatedDto>> ExecuteAsync(
        Guid memberId,
        RegisterProfessionalCertificationDto dto,
        Stream? fileStream = null,
        string? fileName = null,
        string? contentType = null)
    {
        // Validar DTO
        var dtoValidationResult = dto.TryValidateModel();
        if (!dtoValidationResult.WasSuccessful)
            return dtoValidationResult.ChangeNullActionResponseType<RegisterProfessionalCertificationDto, ProfessionalCertificationCreatedDto>();

        // Validar que la institución existe
        var institutionValidation = await dto.InstitutionId.ValidateInstitutionExists(_institutionRepository);
        if (!institutionValidation.WasSuccessful)
            return institutionValidation.ChangeNullActionResponseType<Guid, ProfessionalCertificationCreatedDto>();

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Crear ProfessionalCertification
            var education = dto.ToProfessionalCertification();
            var educationResult = await _professionalCertificationRepository.AddAsync(education);

            if (!educationResult.WasSuccessful)
            {
                await _unitOfWork.RollbackAsync();
                return educationResult.ChangeNullActionResponseType<Domain.Entities.AcademicManagement.ProfessionalCertification, ProfessionalCertificationCreatedDto>();
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
                    return uploadResult.ChangeNullActionResponseType<string, ProfessionalCertificationCreatedDto>();
                }

                memberEducation.DocumentUrl = uploadResult.Result;
            }

            var memberEducationResult = await _memberEducationRepository.AddAsync(memberEducation);

            if (!memberEducationResult.WasSuccessful)
            {
                await _unitOfWork.RollbackAsync();
                return memberEducationResult.ChangeNullActionResponseType<Domain.Entities.AcademicManagement.MemberEducation, ProfessionalCertificationCreatedDto>();
            }

            await _unitOfWork.CommitAsync();

            // Obtener institución para respuesta
            var institution = await _institutionRepository.GetAsync(dto.InstitutionId);

            return ActionResponse<ProfessionalCertificationCreatedDto>.Success(
                memberEducation.ToProfessionalCertificationCreatedDto(education, institution.Result?.Name ?? ""),
                "Certificación profesional registrada correctamente.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ActionResponse<ProfessionalCertificationCreatedDto>.Failure(
                $"Error al registrar la certificación profesional: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
