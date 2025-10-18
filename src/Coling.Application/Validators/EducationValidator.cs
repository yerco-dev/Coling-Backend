using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Interfaces.Repositories.MembersManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.Validators;

public static class EducationValidator
{
    public static async Task<ActionResponse<Guid>> ValidateMemberExists(this Guid memberId, IMemberRepository repository)
    {
        var member = await repository.GetAsync(memberId);

        if (!member.WasSuccessful)
            return ActionResponse<Guid>.NotFound("El miembro no existe.");

        return ActionResponse<Guid>.Success(memberId);
    }

    public static async Task<ActionResponse<Guid>> ValidateInstitutionExists(this Guid institutionId, IInstitutionRepository repository)
    {
        var institution = await repository.GetAsync(institutionId);

        if (!institution.WasSuccessful)
            return ActionResponse<Guid>.NotFound("La institución no existe.");

        return ActionResponse<Guid>.Success(institutionId);
    }

    public static async Task<ActionResponse<MemberEducation>> ValidateDuplicateMemberEducation(
        this Guid memberId,
        Guid educationId,
        IMemberEducationRepository repository)
    {
        var existing = await repository.GetAsync(me =>
            me.MemberId == memberId &&
            me.EducationId == educationId &&
            me.IsActive);

        if (existing.WasSuccessful)
            return ActionResponse<MemberEducation>.Conflict("El miembro ya tiene registrada esta educación.");

        return ActionResponse<MemberEducation>.Success(null);
    }
}
