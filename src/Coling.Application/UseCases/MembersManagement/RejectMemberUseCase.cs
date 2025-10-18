using Coling.Aplication.DTOs.MembersManagement;
using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Aplication.Validators;
using Coling.Application.Mappers.ActionResponse;
using Coling.Domain.Constants;
using Coling.Domain.Wrappers;
using Coling.Domain.Interfaces.Repositories.MembersManagement;
using Coling.Domain.Interfaces.Repositories.UsersManagement;

namespace Coling.Application.UseCases.MembersManagement;

public class RejectMemberUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IDbContextUnitOfWork _dbContextUnitOfWork;

    public RejectMemberUseCase(
        IUserRepository userRepository,
        IMemberRepository memberRepository,
        IDbContextUnitOfWork dbContextUnitOfWork)
    {
        _userRepository = userRepository;
        _memberRepository = memberRepository;
        _dbContextUnitOfWork = dbContextUnitOfWork;
    }

    public async Task<ActionResponse<string>> ExecuteAsync(RejectMemberDto dto)
    {
        var dtoValidationResult = dto.TryValidateModel();
        if (!dtoValidationResult.WasSuccessful)
            return dtoValidationResult.ChangeNullActionResponseType<RejectMemberDto, string>();

        await _dbContextUnitOfWork.BeginTransactionAsync();

        try
        {
            var userResult = await _userRepository.GetFullData(dto.UserId);
            if (!userResult.WasSuccessful)
            {
                await _dbContextUnitOfWork.RollbackAsync();
                return ActionResponse<string>.NotFound("Usuario no encontrado.");
            }

            var user = userResult.Result!;

            var memberResult = await _memberRepository.GetMemberByPersonIdAsync(user.PersonId);
            if (!memberResult.WasSuccessful)
            {
                await _dbContextUnitOfWork.RollbackAsync();
                return ActionResponse<string>.NotFound("No se encontró información de membresía para este usuario.");
            }

            var member = memberResult.Result!;

            var pendingStatus = BusinessConstants.MemberStatusValues[MemberStatus.Pending];
            if (member.Status != pendingStatus)
            {
                await _dbContextUnitOfWork.RollbackAsync();
                return ActionResponse<string>.Conflict($"El miembro ya ha sido procesado. Estado actual: {member.Status}");
            }

            user.IsActive = false;
            member.IsActive = false;

            var updateUserResult = await _userRepository.UpdateAsync(user);
            if (!updateUserResult.WasSuccessful)
            {
                await _dbContextUnitOfWork.RollbackAsync();
                return ActionResponse<string>.Failure("Error al desactivar el usuario.", ResultCode.DatabaseError);
            }

            var updateMemberResult = await _memberRepository.UpdateAsync(member);
            if (!updateMemberResult.WasSuccessful)
            {
                await _dbContextUnitOfWork.RollbackAsync();
                return ActionResponse<string>.Failure("Error al desactivar el miembro.", ResultCode.DatabaseError);
            }

            await _dbContextUnitOfWork.CommitAsync();

            var message = string.IsNullOrWhiteSpace(dto.Reason)
                ? $"Miembro rechazado exitosamente."
                : $"Miembro rechazado. Motivo: {dto.Reason}";

            return ActionResponse<string>.Success(
                message,
                $"El usuario '{user.UserName}' ha sido rechazado y desactivado.");
        }
        catch (Exception ex)
        {
            await _dbContextUnitOfWork.RollbackAsync();
            return ActionResponse<string>.Failure(
                $"Error inesperado al rechazar miembro: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
