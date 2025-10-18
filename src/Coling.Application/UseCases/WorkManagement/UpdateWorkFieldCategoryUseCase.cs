using Coling.Application.DTOs.WorkManagement;
using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Application.Mappers.WorkManagement;
using Coling.Application.Validators;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class UpdateWorkFieldCategoryUseCase
{
    private readonly IWorkFieldCategoryRepository _repository;
    private readonly IDbContextUnitOfWork _unitOfWork;

    public UpdateWorkFieldCategoryUseCase(
        IWorkFieldCategoryRepository repository,
        IDbContextUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ActionResponse<WorkFieldCategoryGetDto>> ExecuteAsync(WorkFieldCategoryUpdateDto dto)
    {
        // Validar existencia
        var categoryResult = await _repository.GetAsync(dto.Id);
        if (!categoryResult.WasSuccessful)
            return ActionResponse<WorkFieldCategoryGetDto>.NotFound(
                "Categoría de campo de trabajo no encontrada.");

        var category = categoryResult.Result!;

        // Validar duplicados (excepto el mismo)
        var existingWithName = await _repository.GetAsync(c =>
            c.Name.ToLower() == dto.Name.ToLower() && c.IsActive && c.Id != dto.Id);

        if (existingWithName.WasSuccessful)
            return ActionResponse<WorkFieldCategoryGetDto>.Conflict(
                "Ya existe otra categoría de campo de trabajo con ese nombre.");

        // Actualizar
        category.UpdateFromDto(dto);

        var updateResult = await _repository.UpdateAsync(category);
        if (!updateResult.WasSuccessful)
            return ActionResponse<WorkFieldCategoryGetDto>.Failure(
                "Error al actualizar la categoría de campo de trabajo.", ResultCode.DatabaseError);

        await _unitOfWork.CommitAsync();

        return ActionResponse<WorkFieldCategoryGetDto>.Success(
            category.ToGetDto(),
            "Categoría de campo de trabajo actualizada exitosamente.");
    }
}
