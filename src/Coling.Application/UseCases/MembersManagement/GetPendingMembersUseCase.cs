using Coling.Aplication.DTOs.MembersManagement;
using Coling.Application.Mappers.ActionResponse;
using Coling.Application.Mappers.MembersManagement;
using Coling.Domain.Wrappers;
using Coling.Domain.Interfaces.Repositories.MembersManagement;
using Coling.Domain.Interfaces.Repositories.UsersManagement;

namespace Coling.Application.UseCases.MembersManagement;

public class GetPendingMembersUseCase
{
    private readonly IMemberRepository _memberRepository;
    private readonly IUserRepository _userRepository;

    public GetPendingMembersUseCase(
        IMemberRepository memberRepository,
        IUserRepository userRepository)
    {
        _memberRepository = memberRepository;
        _userRepository = userRepository;
    }

    public async Task<ActionResponse<IEnumerable<MemberDetailsDto>>> ExecuteAsync()
    {
        var membersResult = await _memberRepository.GetPendingMembersAsync();

        if (!membersResult.WasSuccessful)
            return membersResult.ChangeNullActionResponseType<IEnumerable<Domain.Entities.Member>, IEnumerable<MemberDetailsDto>>();

        var members = membersResult.Result!;

        var memberDetailsList = new List<MemberDetailsDto>();

        foreach (var member in members)
        {
            var userResult = await _userRepository.GetAsync(u => u.PersonId == member.PersonId);

            if (!userResult.WasSuccessful)
                continue; 

            var user = userResult.Result!;

            var memberDetails = member.ToMemberDetailsDto(user, member.Person!);
            memberDetailsList.Add(memberDetails);
        }

        return ActionResponse<IEnumerable<MemberDetailsDto>>.Success(
            memberDetailsList,
            $"Se encontraron {memberDetailsList.Count} miembros pendientes.");
    }
}
