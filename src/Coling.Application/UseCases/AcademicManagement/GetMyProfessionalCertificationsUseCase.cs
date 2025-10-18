using Coling.Application.DTOs.AcademicManagement;
using Coling.Application.Mappers.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.AcademicManagement;

public class GetMyProfessionalCertificationsUseCase
{
    private readonly IMemberEducationRepository _memberEducationRepository;
    private readonly IProfessionalCertificationRepository _professionalCertificationRepository;
    private readonly IInstitutionRepository _institutionRepository;

    public GetMyProfessionalCertificationsUseCase(
        IMemberEducationRepository memberEducationRepository,
        IProfessionalCertificationRepository professionalCertificationRepository,
        IInstitutionRepository institutionRepository)
    {
        _memberEducationRepository = memberEducationRepository;
        _professionalCertificationRepository = professionalCertificationRepository;
        _institutionRepository = institutionRepository;
    }

    public async Task<ActionResponse<IEnumerable<ProfessionalCertificationDetailDto>>> ExecuteAsync(Guid memberId)
    {
        var memberEducationsResult = await _memberEducationRepository.GetByMemberIdWithDetailsAsync(memberId);

        if (!memberEducationsResult.WasSuccessful)
            return ActionResponse<IEnumerable<ProfessionalCertificationDetailDto>>.Failure(
                memberEducationsResult.Message,
                ResultCode.DatabaseError);

        var detailList = new List<ProfessionalCertificationDetailDto>();

        foreach (var memberEducation in memberEducationsResult.Result ?? Enumerable.Empty<Domain.Entities.AcademicManagement.MemberEducation>())
        {
            // Obtener ProfessionalCertification
            var educationResult = await _professionalCertificationRepository.GetAsync(memberEducation.EducationId);

            if (educationResult.WasSuccessful && educationResult.Result != null)
            {
                // Obtener institución
                var institutionResult = await _institutionRepository.GetAsync(educationResult.Result.InstitutionId);
                var institutionName = institutionResult.Result?.Name ?? "";

                detailList.Add(memberEducation.ToProfessionalCertificationDetailDto(educationResult.Result, institutionName));
            }
        }

        return ActionResponse<IEnumerable<ProfessionalCertificationDetailDto>>.Success(
            detailList,
            "Certificaciones profesionales obtenidas correctamente.");
    }
}
