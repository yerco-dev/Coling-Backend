using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class DeleteWorkFieldCategoryUseCase
{
    private readonly IWorkFieldCategoryRepository _repository;
    private readonly IDbContextUnitOfWork _unitOfWork;

    public DeleteWorkFieldCategoryUseCase(
        IWorkFieldCategoryRepository repository,
        IDbContextUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ActionResponse<bool>> ExecuteAsync(Guid id)
    {
        // Validar existencia
        var categoryResult = await _repository.GetAsync(id);
        if (!categoryResult.WasSuccessful)
            return ActionResponse<bool>.NotFound("Categoría de campo de trabajo no encontrada.");

        var category = categoryResult.Result!;

        // Soft delete
        category.IsActive = false;

        var deleteResult = await _repository.UpdateAsync(category);
        if (!deleteResult.WasSuccessful)
            return ActionResponse<bool>.Failure(
                "Error al eliminar la categoría de campo de trabajo.", ResultCode.DatabaseError);

        await _unitOfWork.CommitAsync();

        return ActionResponse<bool>.Success(true, "Categoría de campo de trabajo eliminada exitosamente.");
    }
}
