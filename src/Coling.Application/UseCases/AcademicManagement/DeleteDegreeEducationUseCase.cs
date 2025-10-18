using Coling.Domain.Interfaces.Repositories.AcademicManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.AcademicManagement;

public class DeleteDegreeEducationUseCase
{
    private readonly IMemberEducationRepository _memberEducationRepository;

    public DeleteDegreeEducationUseCase(IMemberEducationRepository memberEducationRepository)
    {
        _memberEducationRepository = memberEducationRepository;
    }

    public async Task<ActionResponse<bool>> ExecuteAsync(Guid memberId, Guid memberEducationId)
    {
        // Obtener MemberEducation
        var memberEducationResult = await _memberEducationRepository.GetAsync(memberEducationId);

        if (!memberEducationResult.WasSuccessful)
            return ActionResponse<bool>.NotFound("Grado académico no encontrado.");

        var memberEducation = memberEducationResult.Result!;

        // Validar que pertenece al miembro autenticado
        if (memberEducation.MemberId != memberId)
            return ActionResponse<bool>.Failure("No tienes permiso para eliminar este grado académico.", ResultCode.Forbidden);

        // Soft delete
        var deleteResult = await _memberEducationRepository.DeleteAsync(memberEducationId);

        if (!deleteResult.WasSuccessful)
            return ActionResponse<bool>.Failure(
                "Error al dar de baja el grado académico.",
                ResultCode.DatabaseError);

        return ActionResponse<bool>.Success(true, "Grado académico dado de baja correctamente.");
    }
}
