using Coling.Application.DTOs.AcademicManagement;
using Coling.Application.Mappers.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.AcademicManagement;

public class GetMyContinuingEducationsUseCase
{
    private readonly IMemberEducationRepository _memberEducationRepository;
    private readonly IContinuingEducationRepository _continuingEducationRepository;
    private readonly IInstitutionRepository _institutionRepository;

    public GetMyContinuingEducationsUseCase(
        IMemberEducationRepository memberEducationRepository,
        IContinuingEducationRepository continuingEducationRepository,
        IInstitutionRepository institutionRepository)
    {
        _memberEducationRepository = memberEducationRepository;
        _continuingEducationRepository = continuingEducationRepository;
        _institutionRepository = institutionRepository;
    }

    public async Task<ActionResponse<IEnumerable<ContinuingEducationDetailDto>>> ExecuteAsync(Guid memberId)
    {
        var memberEducationsResult = await _memberEducationRepository.GetByMemberIdWithDetailsAsync(memberId);

        if (!memberEducationsResult.WasSuccessful)
            return ActionResponse<IEnumerable<ContinuingEducationDetailDto>>.Failure(
                memberEducationsResult.Message,
                ResultCode.DatabaseError);

        var detailList = new List<ContinuingEducationDetailDto>();

        foreach (var memberEducation in memberEducationsResult.Result ?? Enumerable.Empty<Domain.Entities.AcademicManagement.MemberEducation>())
        {
            // Obtener ContinuingEducation
            var educationResult = await _continuingEducationRepository.GetAsync(memberEducation.EducationId);

            if (educationResult.WasSuccessful && educationResult.Result != null)
            {
                // Obtener institución
                var institutionResult = await _institutionRepository.GetAsync(educationResult.Result.InstitutionId);
                var institutionName = institutionResult.Result?.Name ?? "";

                detailList.Add(memberEducation.ToContinuingEducationDetailDto(educationResult.Result, institutionName));
            }
        }

        return ActionResponse<IEnumerable<ContinuingEducationDetailDto>>.Success(
            detailList,
            "Educaciones continuas obtenidas correctamente.");
    }
}
