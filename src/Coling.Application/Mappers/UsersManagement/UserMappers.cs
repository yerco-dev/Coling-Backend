using Coling.Domain.Entities.UsersManagement;
using Coling.Domain.Entities;
using Coling.Domain.Interfaces.Repositories.UsersManagement;
using Coling.Aplication.DTOs.UsersManagement;

namespace Coling.Application.Mappers.UsersManagement;

public static class UserMappers
{
    public static User ToUser(this RegisterMemberUserDto dto, Guid personId)
    {
        return new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PersonId = personId
        };
    }

    public static RegisterMemberUserDto ToRegisterMemberUserDto(this RegisterByAdminDto dto, string password)
    {
        return new RegisterMemberUserDto
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FirstNames = dto.FirstNames,
            MaternalLastName = dto.MaternalLastName,
            PaternalLastName = dto.PaternalLastName,
            Password = password
        };
    }

    public static UserCreatedDto ToUserCreatedDto(this User user, Person person, string role)
    {
        return new UserCreatedDto
        {
            UserName = user.UserName ?? "",
            FirstNames = person.FirstNames,
            MaternalLastName = person.MaternalLastName,
            PaternalLastName = person.PaternalLastName,
            Email = user.Email,
            Role = role
        };
    }

    public static UserLogedDataDto ToLogedDto(this User user, string role, string token)
    {
        return new UserLogedDataDto
        {
            Id = user.Id,
            PersonId = user.PersonId,
            UserName = user.UserName!,
            Email = user.Email!,
            FirstNames = user.Person!.FirstNames,
            FullName = user.Person.FullName,
            Role = role,
            Token = token
        };
    }

    public static LoginResponseDto ToLoginResponseDto(this User user, string role, string token)
    {
        return new LoginResponseDto
        {
            UserId = user.Id,
            PersonId = user.PersonId,
            UserName = user.UserName!,
            Email = user.Email!,
            FirstNames = user.Person!.FirstNames,
            FullName = user.Person.FullName,
            Role = role,
            Token = token
        };
    }

    public static UserGetDto ToGetDto(this User user, IUserRepository userRepository)
    {
        var roles = userRepository.GetRolesAsync(user);
        return new UserGetDto
        {
            Id = user.Id,
            UserName = user.UserName ?? "",
            Role = roles.Result.Count > 0 ? roles.Result[0] : "",
            Email = user.Email ?? "",
            FullName = user.Person!.FullName,
            isActive = user.IsActive,
        };
    }
}
