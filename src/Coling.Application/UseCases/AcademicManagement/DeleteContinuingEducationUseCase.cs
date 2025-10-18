using Coling.Domain.Interfaces.Repositories.AcademicManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.AcademicManagement;

public class DeleteContinuingEducationUseCase
{
    private readonly IMemberEducationRepository _memberEducationRepository;

    public DeleteContinuingEducationUseCase(IMemberEducationRepository memberEducationRepository)
    {
        _memberEducationRepository = memberEducationRepository;
    }

    public async Task<ActionResponse<bool>> ExecuteAsync(Guid memberId, Guid memberEducationId)
    {
        // Obtener MemberEducation
        var memberEducationResult = await _memberEducationRepository.GetAsync(memberEducationId);

        if (!memberEducationResult.WasSuccessful)
            return ActionResponse<bool>.NotFound("Educación continua no encontrada.");

        var memberEducation = memberEducationResult.Result!;

        // Validar que pertenece al miembro autenticado
        if (memberEducation.MemberId != memberId)
            return ActionResponse<bool>.Failure("No tienes permiso para eliminar esta educación continua.", ResultCode.Forbidden);

        // Soft delete
        var deleteResult = await _memberEducationRepository.DeleteAsync(memberEducationId);

        if (!deleteResult.WasSuccessful)
            return ActionResponse<bool>.Failure(
                "Error al dar de baja la educación continua.",
                ResultCode.DatabaseError);

        return ActionResponse<bool>.Success(true, "Educación continua dada de baja correctamente.");
    }
}
