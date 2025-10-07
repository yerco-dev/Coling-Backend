using Coling.Aplication.DTOs.UsersManagement;
using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Domain.Entities.ActionResponse;
using Coling.Domain.Entities.UsersManagement;
using Coling.Domain.Entities;
using Coling.Domain.Interfaces.Repositories.UsersManagement;
using Coling.Aplication.Validators;
using Coling.Application.Mappers.ActionResponse;
using Coling.Domain.Interfaces.Repositories.PeopleManagement;
using Coling.Domain.Interfaces.Repositories.MembersManagement;
using Coling.Application.Mappers.PeopleManagement;
using Coling.Application.Mappers.UsersManagement;
using Coling.Application.Mappers.MembersManagement;
using Coling.Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Coling.Application.UseCases.UsersManagement;


public class RegisterMemberUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IDbContextUnitOfWork _dbContextUnitOfWork;
    private readonly UserManager<User> _userManager;

    public RegisterMemberUserUseCase(IUserRepository userRepository,
                               IPersonRepository personRepository,
                               IDbContextUnitOfWork dbContextUnitOfWork,
                               IMemberRepository memberRepository,
                               UserManager<User> userManager)
    {
        _userRepository = userRepository;
        _personRepository = personRepository;
        _memberRepository = memberRepository;
        _dbContextUnitOfWork = dbContextUnitOfWork;
        _userManager = userManager;
    }

    public async Task<ActionResponse<UserCreatedDto>> ExecuteAsync(RegisterMemberUserDto dto)
    {
        var dtoValidationResult = dto.TryValidateModel();
        if (!dtoValidationResult.WasSuccessful)
            return dtoValidationResult.ChangeNullActionResponseType<RegisterMemberUserDto, UserCreatedDto>();

        Person person = dto.ToPerson();
        var modelValidationResult = person.TryValidateModel();
        if (!modelValidationResult.WasSuccessful)
            return modelValidationResult.ChangeNullActionResponseType<Person, UserCreatedDto>();

        var uniqueUserNameValidationResult = await dto.UserName.ValidateUniqueUserName(_userRepository);
        if (!uniqueUserNameValidationResult.WasSuccessful)
            return uniqueUserNameValidationResult.ChangeNullActionResponseType<User, UserCreatedDto>();

        await _dbContextUnitOfWork.BeginTransactionAsync();

        try
        {
            var createPersonResult = await _personRepository.AddAsync(person);

            if (!createPersonResult.WasSuccessful)
            {
                await _dbContextUnitOfWork.RollbackAsync();
                return createPersonResult.ChangeNullActionResponseType<Person, UserCreatedDto>();
            }

            Member member = dto.ToMember(createPersonResult.Result!.Id);
            member.Status = BusinessConstants.MemberStatusValues[MemberStatus.Pending];

            var createMemberResult = await _memberRepository.AddAsync(member);

            if (!createMemberResult.WasSuccessful)
            {
                await _dbContextUnitOfWork.RollbackAsync();
                return createMemberResult.ChangeNullActionResponseType<Member, UserCreatedDto>();
            }

            User user = dto.ToUser(createPersonResult.Result!.Id);

            var createUserResult = await _userRepository.AddAsync(user, dto.Password);

            if (!createUserResult.WasSuccessful)
            {
                await _dbContextUnitOfWork.RollbackAsync();
                return createUserResult.ChangeNullActionResponseType<User, UserCreatedDto>();
            }

            var role = BusinessConstants.SystemRolesValues[SystemRoles.Member];

            // Asignar rol usando UserManager directamente (lógica de negocio en UseCase)
            var assignRoleResult = await _userManager.AddToRoleAsync(createUserResult.Result!, role);

            if (!assignRoleResult.Succeeded)
            {
                await _dbContextUnitOfWork.RollbackAsync();
                return new ActionResponse<UserCreatedDto>
                {
                    WasSuccessful = false,
                    Message = $"Error al asignar rol: {string.Join(", ", assignRoleResult.Errors.Select(e => e.Description))}",
                    ResultCode = ResultCode.DatabaseError
                };
            }

            await _dbContextUnitOfWork.CommitAsync();

            return new ActionResponse<UserCreatedDto>
            {
                WasSuccessful = true,
                Message = "Usuario registrado correctamente.",
                ResultCode = ResultCode.Ok,
                Result = createUserResult.Result!.ToUserCreatedDto(createPersonResult.Result!, role)
            };
        }
        catch (Exception ex)
        {
            await _dbContextUnitOfWork.RollbackAsync();

            return new ActionResponse<UserCreatedDto>
            {
                WasSuccessful = false,
                Message = "Error inesperado al guardar los registros del usuario: " + ex.Message,
                ResultCode = ResultCode.DatabaseError,
            };
        }
    }
}