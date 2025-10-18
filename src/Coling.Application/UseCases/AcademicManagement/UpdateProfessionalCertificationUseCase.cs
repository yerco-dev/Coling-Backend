using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Aplication.Validators;
using Coling.Application.DTOs.AcademicManagement;
using Coling.Application.Interfaces.Services.Storage;
using Coling.Application.Mappers.ActionResponse;
using Coling.Application.Mappers.AcademicManagement;
using Coling.Application.Validators;
using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Entities.PartialDateManagement;
using Coling.Domain.Interfaces.Repositories.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.AcademicManagement;

public class UpdateProfessionalCertificationUseCase
{
    private readonly IMemberEducationRepository _memberEducationRepository;
    private readonly IProfessionalCertificationRepository _professionalCertificationRepository;
    private readonly IInstitutionRepository _institutionRepository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IDbContextUnitOfWork _unitOfWork;

    public UpdateProfessionalCertificationUseCase(
        IMemberEducationRepository memberEducationRepository,
        IProfessionalCertificationRepository professionalCertificationRepository,
        IInstitutionRepository institutionRepository,
        IBlobStorageService blobStorageService,
        IDbContextUnitOfWork unitOfWork)
    {
        _memberEducationRepository = memberEducationRepository;
        _professionalCertificationRepository = professionalCertificationRepository;
        _institutionRepository = institutionRepository;
        _blobStorageService = blobStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ActionResponse<ProfessionalCertificationDetailDto>> ExecuteAsync(
        Guid memberId,
        Guid memberEducationId,
        UpdateProfessionalCertificationDto dto,
        Stream? fileStream = null,
        string? fileName = null,
        string? contentType = null)
    {
        // Validar DTO
        var dtoValidationResult = dto.TryValidateModel();
        if (!dtoValidationResult.WasSuccessful)
            return dtoValidationResult.ChangeNullActionResponseType<UpdateProfessionalCertificationDto, ProfessionalCertificationDetailDto>();

        // Validar que la institución existe
        var institutionValidation = await dto.InstitutionId.ValidateInstitutionExists(_institutionRepository);
        if (!institutionValidation.WasSuccessful)
            return institutionValidation.ChangeNullActionResponseType<Guid, ProfessionalCertificationDetailDto>();

        // Obtener MemberEducation
        var memberEducationResult = await _memberEducationRepository.GetAsync(memberEducationId);
        if (!memberEducationResult.WasSuccessful)
            return ActionResponse<ProfessionalCertificationDetailDto>.NotFound("Certificación profesional no encontrada.");

        var memberEducation = memberEducationResult.Result!;

        // Validar que pertenece al miembro autenticado
        if (memberEducation.MemberId != memberId)
            return ActionResponse<ProfessionalCertificationDetailDto>.Failure("No tienes permiso para editar esta certificación profesional.", ResultCode.Forbidden);

        // Obtener ProfessionalCertification
        var educationResult = await _professionalCertificationRepository.GetAsync(memberEducation.EducationId);
        if (!educationResult.WasSuccessful)
            return ActionResponse<ProfessionalCertificationDetailDto>.NotFound("Certificación no encontrada.");

        var education = educationResult.Result!;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Actualizar ProfessionalCertification
            education.Name = dto.Name;
            education.Description = dto.Description;
            education.InstitutionId = dto.InstitutionId;
            education.CertificationNumber = dto.CertificationNumber;
            education.ExpirationDate = dto.ExpirationDate;
            education.RequiresRenewal = dto.RequiresRenewal;

            var updateEducationResult = await _professionalCertificationRepository.UpdateAsync(education);
            if (!updateEducationResult.WasSuccessful)
            {
                await _unitOfWork.RollbackAsync();
                return updateEducationResult.ChangeNullActionResponseType<ProfessionalCertification, ProfessionalCertificationDetailDto>();
            }

            // Subir nuevo archivo si existe
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
                    return uploadResult.ChangeNullActionResponseType<string, ProfessionalCertificationDetailDto>();
                }

                // Eliminar archivo anterior si existe
                if (!string.IsNullOrEmpty(memberEducation.DocumentUrl))
                {
                    var oldFileName = Path.GetFileName(new Uri(memberEducation.DocumentUrl).LocalPath);
                    await _blobStorageService.DeleteFileAsync("academic-documents", oldFileName);
                }

                memberEducation.DocumentUrl = uploadResult.Result;
            }

            // Actualizar MemberEducation
            memberEducation.TitleReceived = dto.TitleReceived;
            memberEducation.Status = dto.Status;

            // Actualizar fechas
            if (dto.StartYear.HasValue)
            {
                memberEducation.StartDate = new PartialDate(dto.StartYear.Value, dto.StartMonth, dto.StartDay);
            }
            else
            {
                memberEducation.StartDate = null;
            }

            if (dto.EndYear.HasValue)
            {
                memberEducation.EndDate = new PartialDate(dto.EndYear.Value, dto.EndMonth, dto.EndDay);
            }
            else
            {
                memberEducation.EndDate = null;
            }

            var updateMemberEducationResult = await _memberEducationRepository.UpdateAsync(memberEducation);
            if (!updateMemberEducationResult.WasSuccessful)
            {
                await _unitOfWork.RollbackAsync();
                return updateMemberEducationResult.ChangeNullActionResponseType<MemberEducation, ProfessionalCertificationDetailDto>();
            }

            await _unitOfWork.CommitAsync();

            // Obtener institución para respuesta
            var institution = await _institutionRepository.GetAsync(dto.InstitutionId);

            return ActionResponse<ProfessionalCertificationDetailDto>.Success(
                memberEducation.ToProfessionalCertificationDetailDto(education, institution.Result?.Name ?? ""),
                "Certificación profesional actualizada correctamente.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ActionResponse<ProfessionalCertificationDetailDto>.Failure(
                $"Error al actualizar la certificación profesional: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
