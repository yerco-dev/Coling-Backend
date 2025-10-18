using Coling.Aplication.DTOs.MembersManagement;
using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Aplication.Validators;
using Coling.Application.Mappers.ActionResponse;
using Coling.Domain.Constants;
using Coling.Domain.Wrappers;
using Coling.Domain.Interfaces.Repositories.MembersManagement;
using Coling.Domain.Interfaces.Repositories.UsersManagement;

namespace Coling.Application.UseCases.MembersManagement;

public class ApproveMemberUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IDbContextUnitOfWork _dbContextUnitOfWork;

    public ApproveMemberUseCase(
        IUserRepository userRepository,
        IMemberRepository memberRepository,
        IDbContextUnitOfWork dbContextUnitOfWork)
    {
        _userRepository = userRepository;
        _memberRepository = memberRepository;
        _dbContextUnitOfWork = dbContextUnitOfWork;
    }

    public async Task<ActionResponse<string>> ExecuteAsync(ApproveMemberDto dto)
    {
        // 1. Validar DTO
        var dtoValidationResult = dto.TryValidateModel();
        if (!dtoValidationResult.WasSuccessful)
            return dtoValidationResult.ChangeNullActionResponseType<ApproveMemberDto, string>();

        await _dbContextUnitOfWork.BeginTransactionAsync();

        try
        {
            // 2. Obtener usuario
            var userResult = await _userRepository.GetFullData(dto.UserId);
            if (!userResult.WasSuccessful)
            {
                await _dbContextUnitOfWork.RollbackAsync();
                return ActionResponse<string>.NotFound("Usuario no encontrado.");
            }

            var user = userResult.Result!;

            // 3. Verificar que el usuario esté activo
            if (!user.IsActive)
            {
                await _dbContextUnitOfWork.RollbackAsync();
                return ActionResponse<string>.Failure("El usuario está desactivado.", ResultCode.Conflict);
            }

            // 4. Obtener miembro asociado
            var memberResult = await _memberRepository.GetMemberByPersonIdAsync(user.PersonId);
            if (!memberResult.WasSuccessful)
            {
                await _dbContextUnitOfWork.RollbackAsync();
                return ActionResponse<string>.NotFound("No se encontró información de membresía para este usuario.");
            }

            var member = memberResult.Result!;

            // 5. Verificar que el miembro esté pendiente
            var pendingStatus = BusinessConstants.MemberStatusValues[MemberStatus.Pending];
            if (member.Status != pendingStatus)
            {
                await _dbContextUnitOfWork.RollbackAsync();
                return ActionResponse<string>.Conflict($"El miembro ya ha sido procesado. Estado actual: {member.Status}");
            }

            // 6. Aprobar miembro (cambiar estado)
            member.Status = BusinessConstants.MemberStatusValues[MemberStatus.Verified];

            var updateResult = await _memberRepository.UpdateAsync(member);
            if (!updateResult.WasSuccessful)
            {
                await _dbContextUnitOfWork.RollbackAsync();
                return ActionResponse<string>.Failure("Error al actualizar el estado del miembro.", ResultCode.DatabaseError);
            }

            await _dbContextUnitOfWork.CommitAsync();

            return ActionResponse<string>.Success(
                $"Miembro aprobado exitosamente.",
                $"El usuario '{user.UserName}' ha sido verificado como miembro.");
        }
        catch (Exception ex)
        {
            await _dbContextUnitOfWork.RollbackAsync();
            return ActionResponse<string>.Failure(
                $"Error inesperado al aprobar miembro: {ex.Message}",
                ResultCode.DatabaseError);
        }
    }
}
