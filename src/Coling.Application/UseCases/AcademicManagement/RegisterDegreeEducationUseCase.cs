using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Aplication.Validators;
using Coling.Application.DTOs.AcademicManagement;
using Coling.Application.Interfaces.Services.Storage;
using Coling.Application.Mappers.AcademicManagement;
using Coling.Application.Mappers.ActionResponse;
using Coling.Application.Validators;
using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Interfaces.Repositories.MembersManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.AcademicManagement;

public class RegisterDegreeEducationUseCase
{
    private readonly IDegreeEducationRepository _degreeEducationRepository;
    private readonly IMemberEducationRepository _memberEducationRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IInstitutionRepository _institutionRepository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IDbContextUnitOfWork _unitOfWork;

    public RegisterDegreeEducationUseCase(
        IDegreeEducationRepository degreeEducationRepository,
        IMemberEducationRepository memberEducationRepository,
        IMemberRepository memberRepository,
        IInstitutionRepository institutionRepository,
        IBlobStorageService blobStorageService,
        IDbContextUnitOfWork unitOfWork)
    {
        _degreeEducationRepository = degreeEducationRepository;
        _memberEducationRepository = memberEducationRepository;
        _memberRepository = memberRepository;
        _institutionRepository = institutionRepository;
        _blobStorageService = blobStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ActionResponse<MemberEducationCreatedDto>> ExecuteAsync(
        Guid memberId,
        RegisterDegreeEducationDto dto,
        Stream? fileStream = null,
        string? fileName = null,
        string? contentType = null)
    {
        var dtoValidationResult = dto.TryValidateModel();
        if (!dtoValidationResult.WasSuccessful)
            return dtoValidationResult.ChangeNullActionResponseType<RegisterDegreeEducationDto, MemberEducationCreatedDto>();

        var memberValidation = await memberId.ValidateMemberExists(_memberRepository);
        if (!memberValidation.WasSuccessful)
            return memberValidation.ChangeNullActionResponseType<Guid, MemberEducationCreatedDto>();

        var institutionValidation = await dto.InstitutionId.ValidateInstitutionExists(_institutionRepository);
        if (!institutionValidation.WasSuccessful)
            return institutionValidation.ChangeNullActionResponseType<Guid, MemberEducationCreatedDto>();

        DegreeEducation degreeEducation = dto.ToDegreeEducation();
        var modelValidation = degreeEducation.TryValidateModel();
        if (!modelValidation.WasSuccessful)
            return modelValidation.ChangeNullActionResponseType<DegreeEducation, MemberEducationCreatedDto>();

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var createEducationResult = await _degreeEducationRepository.AddAsync(degreeEducation);

            if (!createEducationResult.WasSuccessful)
            {
                await _unitOfWork.RollbackAsync();
                return createEducationResult.ChangeNullActionResponseType<DegreeEducation, MemberEducationCreatedDto>();
            }

            var duplicateValidation = await memberId.ValidateDuplicateMemberEducation(
                createEducationResult.Result!.Id,
                _memberEducationRepository);

            if (!duplicateValidation.WasSuccessful)
            {
                await _unitOfWork.RollbackAsync();
                return duplicateValidation.ChangeNullActionResponseType<MemberEducation, MemberEducationCreatedDto>();
            }

            string? documentUrl = null;
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
                    return uploadResult.ChangeNullActionResponseType<string, MemberEducationCreatedDto>();
                }

                documentUrl = uploadResult.Result;
            }

            MemberEducation memberEducation = dto.ToMemberEducation(memberId, createEducationResult.Result!.Id);
            memberEducation.DocumentUrl = documentUrl; 
            var createMemberEducationResult = await _memberEducationRepository.AddAsync(memberEducation);

            if (!createMemberEducationResult.WasSuccessful)
            {
                await _unitOfWork.RollbackAsync();
                return createMemberEducationResult.ChangeNullActionResponseType<MemberEducation, MemberEducationCreatedDto>();
            }

            await _unitOfWork.CommitAsync();

            var institution = await _institutionRepository.GetAsync(dto.InstitutionId);

            return ActionResponse<MemberEducationCreatedDto>.Success(
                createMemberEducationResult.Result!.ToMemberEducationCreatedDto(
                    createEducationResult.Result!,
                    institution.Result?.Name ?? ""),
                "Grado académico registrado correctamente.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            return ActionResponse<MemberEducationCreatedDto>.Failure(
                $"Error inesperado al registrar el grado académico: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
