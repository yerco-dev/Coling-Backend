using Coling.Domain.Entities;
using Coling.Domain.Wrappers;
using Coling.Domain.Interfaces.Repositories.MembersManagement;

namespace Coling.Application.Validators;

public static class MemberValidator
{
    public static async Task<ActionResponse<Member>> ValidateUniqueMembershipCode(this string membershipCode, IMemberRepository repository, Guid? omitedGuid = null)
    {
        var existingMember = await repository.GetAsync(m => m.MembershipCode == membershipCode);

        if (existingMember.WasSuccessful && (omitedGuid == null || existingMember.Result!.Id != omitedGuid))
            return ActionResponse<Member>.Conflict("El código de carnet ya se encuentra registrado en el sistema.");

        return ActionResponse<Member>.Success(existingMember.Result);
    }

    public static async Task<ActionResponse<Member>> ValidateUniqueTitleNumber(this string titleNumber, IMemberRepository repository, Guid? omitedGuid = null)
    {
        var existingMember = await repository.GetAsync(m => m.TitleNumber == titleNumber);

        if (existingMember.WasSuccessful && (omitedGuid == null || existingMember.Result!.Id != omitedGuid))
            return ActionResponse<Member>.Conflict("El número de título ya se encuentra registrado en el sistema.");

        return ActionResponse<Member>.Success(existingMember.Result);
    }
}
