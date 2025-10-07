using Coling.Aplication.DTOs.UsersManagement;
using Coling.Domain.Entities;

namespace Coling.Application.Mappers.PeopleManagement;

public static class PersonMappers
{
    public static Person ToPerson(this RegisterMemberUserDto dto)
    {
        return new Person()
        {
            NationalId = dto.NationalId,
            FirstNames = dto.FirstNames,
            PaternalLastName = dto.PaternalLastName,
            MaternalLastName = dto.MaternalLastName
        };
    }
}
