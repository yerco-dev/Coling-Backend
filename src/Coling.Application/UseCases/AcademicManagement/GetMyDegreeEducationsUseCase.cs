using Coling.Application.DTOs.AcademicManagement;
using Coling.Application.Mappers.AcademicManagement;
using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.AcademicManagement;

public class GetMyDegreeEducationsUseCase
{
    private readonly IMemberEducationRepository _memberEducationRepository;
    private readonly IDegreeEducationRepository _degreeEducationRepository;
    private readonly IInstitutionRepository _institutionRepository;

    public GetMyDegreeEducationsUseCase(
        IMemberEducationRepository memberEducationRepository,
        IDegreeEducationRepository degreeEducationRepository,
        IInstitutionRepository institutionRepository)
    {
        _memberEducationRepository = memberEducationRepository;
        _degreeEducationRepository = degreeEducationRepository;
        _institutionRepository = institutionRepository;
    }

    public async Task<ActionResponse<IEnumerable<DegreeEducationDetailDto>>> ExecuteAsync(Guid memberId)
    {
        // Obtener todas las educaciones del miembro
        var memberEducationsResult = await _memberEducationRepository.GetByMemberIdWithDetailsAsync(memberId);

        if (!memberEducationsResult.WasSuccessful)
            return ActionResponse<IEnumerable<DegreeEducationDetailDto>>.Failure(
                memberEducationsResult.Message,
                memberEducationsResult.ResultCode);

        var memberEducations = memberEducationsResult.Result ?? new List<MemberEducation>();
        var degreeEducationDetails = new List<DegreeEducationDetailDto>();

        foreach (var memberEducation in memberEducations)
        {
            // Obtener el DegreeEducation
            var educationResult = await _degreeEducationRepository.GetAsync(memberEducation.EducationId);

            if (!educationResult.WasSuccessful || educationResult.Result == null)
                continue;

            var education = educationResult.Result;

            // Obtener la institución
            var institutionResult = await _institutionRepository.GetAsync(education.InstitutionId);
            var institutionName = institutionResult.Result?.Name ?? "";

            // Mapear a DTO
            var detailDto = memberEducation.ToDegreeEducationDetailDto(education, institutionName);
            degreeEducationDetails.Add(detailDto);
        }

        return ActionResponse<IEnumerable<DegreeEducationDetailDto>>.Success(
            degreeEducationDetails,
            "Grados académicos obtenidos exitosamente.");
    }
}
