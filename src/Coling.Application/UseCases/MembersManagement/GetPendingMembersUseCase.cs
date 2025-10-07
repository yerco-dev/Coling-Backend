using Coling.Aplication.DTOs.MembersManagement;
using Coling.Application.Mappers.ActionResponse;
using Coling.Application.Mappers.MembersManagement;
using Coling.Domain.Entities.ActionResponse;
using Coling.Domain.Interfaces.Repositories.MembersManagement;
using Coling.Domain.Interfaces.Repositories.UsersManagement;
using Microsoft.EntityFrameworkCore;

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
        // 1. Obtener miembros pendientes
        var membersResult = await _memberRepository.GetPendingMembersAsync();

        if (!membersResult.WasSuccessful)
            return membersResult.ChangeNullActionResponseType<IEnumerable<Domain.Entities.Member>, IEnumerable<MemberDetailsDto>>();

        var members = membersResult.Result!;

        // 2. Mapear a DTOs con información completa
        var memberDetailsList = new List<MemberDetailsDto>();

        foreach (var member in members)
        {
            // Obtener usuario asociado
            var userResult = await _userRepository.GetAsync(u => u.PersonId == member.PersonId);

            if (!userResult.WasSuccessful)
                continue; // Saltar si no se encuentra el usuario

            var user = userResult.Result!;

            // Mapear a DTO
            var memberDetails = member.ToMemberDetailsDto(user, member.Person!);
            memberDetailsList.Add(memberDetails);
        }

        return ActionResponse<IEnumerable<MemberDetailsDto>>.Success(
            memberDetailsList,
            $"Se encontraron {memberDetailsList.Count} miembros pendientes.");
    }
}
