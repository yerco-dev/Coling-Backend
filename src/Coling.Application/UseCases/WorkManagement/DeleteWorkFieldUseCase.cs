using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class DeleteWorkFieldUseCase
{
    private readonly IWorkFieldRepository _repository;
    private readonly IDbContextUnitOfWork _unitOfWork;

    public DeleteWorkFieldUseCase(
        IWorkFieldRepository repository,
        IDbContextUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ActionResponse<bool>> ExecuteAsync(Guid id)
    {
        // Validar existencia
        var workFieldResult = await _repository.GetAsync(id);
        if (!workFieldResult.WasSuccessful)
            return ActionResponse<bool>.NotFound("Campo de trabajo no encontrado.");

        var workField = workFieldResult.Result!;

        // Soft delete
        workField.IsActive = false;

        var deleteResult = await _repository.UpdateAsync(workField);
        if (!deleteResult.WasSuccessful)
            return ActionResponse<bool>.Failure(
                "Error al eliminar el campo de trabajo.", ResultCode.DatabaseError);

        await _unitOfWork.CommitAsync();

        return ActionResponse<bool>.Success(true, "Campo de trabajo eliminado exitosamente.");
    }
}
