using Coling.Aplication.DTOs.MembersManagement;
using Coling.Domain.Entities;
using Coling.Domain.Entities.UsersManagement;

namespace Coling.Application.Mappers.MembersManagement;

public static class MemberDetailsMappers
{
    public static MemberDetailsDto ToMemberDetailsDto(this Member member, User user, Person person)
    {
        return new MemberDetailsDto
        {
            MemberId = member.Id,
            UserId = user.Id,
            PersonId = person.Id,

            // Datos del usuario
            UserName = user.UserName ?? "",
            Email = user.Email ?? "",
            IsActive = user.IsActive,

            // Datos personales
            NationalId = person.NationalId,
            FirstNames = person.FirstNames,
            PaternalLastName = person.PaternalLastName,
            MaternalLastName = person.MaternalLastName,
            FullName = person.FullName,
            BirthDate = person.BirthDate,
            PhotoUrl = person.PhotoUrl,

            // Datos de membresía
            MembershipDate = member.MembershipDate,
            MembershipCode = member.MembershipCode,
            TitleNumber = member.TitleNumber,
            Status = member.Status
        };
    }
}
