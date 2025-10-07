using Coling.Aplication.DTOs.UsersManagement;
using Coling.Domain.Entities;

namespace Coling.Application.Mappers.MembersManagement;

public static class MemberMappers
{
    public static Member ToMember(this RegisterMemberUserDto dto, Guid personId)
    {
        return new Member()
        {
            MembershipCode = dto.MembershipCode,
            MembershipDate = dto.MembershipDate,
            TitleNumber = dto.TitleNumber,
            PersonId = personId
        };
    }
}
