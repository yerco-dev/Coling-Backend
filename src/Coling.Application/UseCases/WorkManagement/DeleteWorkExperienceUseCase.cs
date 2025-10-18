using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class DeleteWorkExperienceUseCase
{
    private readonly IWorkExperienceRepository _workExperienceRepository;
    private readonly IDbContextUnitOfWork _unitOfWork;

    public DeleteWorkExperienceUseCase(
        IWorkExperienceRepository workExperienceRepository,
        IDbContextUnitOfWork unitOfWork)
    {
        _workExperienceRepository = workExperienceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ActionResponse<bool>> ExecuteAsync(Guid id, Guid memberId)
    {
        var workExperienceResult = await _workExperienceRepository.GetAsync(id);
        if (!workExperienceResult.WasSuccessful)
            return ActionResponse<bool>.NotFound("Experiencia laboral no encontrada.");

        var workExperience = workExperienceResult.Result!;

        // Validar ownership
        if (workExperience.MemberId != memberId)
            return ActionResponse<bool>.Failure("No tienes permiso para eliminar esta experiencia laboral.");

        // Soft delete
        workExperience.IsActive = false;

        var deleteResult = await _workExperienceRepository.UpdateAsync(workExperience);
        if (!deleteResult.WasSuccessful)
            return ActionResponse<bool>.Failure(
                "Error al eliminar la experiencia laboral.",
                ResultCode.DatabaseError);

        await _unitOfWork.CommitAsync();

        return ActionResponse<bool>.Success(true, "Experiencia laboral eliminada correctamente.");
    }
}
