using Coling.Application.DTOs.WorkManagement;
using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Application.Mappers.WorkManagement;
using Coling.Application.Validators;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class UpdateWorkFieldUseCase
{
    private readonly IWorkFieldRepository _workFieldRepository;
    private readonly IWorkFieldCategoryRepository _categoryRepository;
    private readonly IDbContextUnitOfWork _unitOfWork;

    public UpdateWorkFieldUseCase(
        IWorkFieldRepository workFieldRepository,
        IWorkFieldCategoryRepository categoryRepository,
        IDbContextUnitOfWork unitOfWork)
    {
        _workFieldRepository = workFieldRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ActionResponse<WorkFieldGetDto>> ExecuteAsync(WorkFieldUpdateDto dto)
    {
        // Validar existencia del work field
        var workFieldResult = await _workFieldRepository.GetAsync(dto.Id);
        if (!workFieldResult.WasSuccessful)
            return ActionResponse<WorkFieldGetDto>.NotFound("Campo de trabajo no encontrado.");

        var workField = workFieldResult.Result!;

        // Validar que la categoría existe
        var categoryValidation = await dto.WorkFieldCategoryId.ValidateWorkFieldCategoryExists(_categoryRepository);
        if (!categoryValidation.WasSuccessful)
            return ActionResponse<WorkFieldGetDto>.NotFound(categoryValidation.Message!);

        // Validar duplicados (excepto el mismo)
        var existingWithName = await _workFieldRepository.GetAsync(wf =>
            wf.Name.ToLower() == dto.Name.ToLower() && wf.IsActive && wf.Id != dto.Id);

        if (existingWithName.WasSuccessful)
            return ActionResponse<WorkFieldGetDto>.Conflict("Ya existe otro campo de trabajo con ese nombre.");

        // Actualizar
        workField.UpdateFromDto(dto);

        var updateResult = await _workFieldRepository.UpdateAsync(workField);
        if (!updateResult.WasSuccessful)
            return ActionResponse<WorkFieldGetDto>.Failure(
                "Error al actualizar el campo de trabajo.", ResultCode.DatabaseError);

        await _unitOfWork.CommitAsync();

        // Obtener con la categoría incluida
        var workFieldWithCategory = await _workFieldRepository.GetByIdWithCategoryAsync(workField.Id);
        if (workFieldWithCategory.WasSuccessful)
            return ActionResponse<WorkFieldGetDto>.Success(
                workFieldWithCategory.Result!.ToGetDto(),
                "Campo de trabajo actualizado exitosamente.");

        return ActionResponse<WorkFieldGetDto>.Success(
            workField.ToGetDto(),
            "Campo de trabajo actualizado exitosamente.");
    }
}
