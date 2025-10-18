using Coling.Application.DTOs.WorkManagement;
using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Application.Mappers.WorkManagement;
using Coling.Application.Validators;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class CreateWorkFieldUseCase
{
    private readonly IWorkFieldRepository _workFieldRepository;
    private readonly IWorkFieldCategoryRepository _categoryRepository;
    private readonly IDbContextUnitOfWork _unitOfWork;

    public CreateWorkFieldUseCase(
        IWorkFieldRepository workFieldRepository,
        IWorkFieldCategoryRepository categoryRepository,
        IDbContextUnitOfWork unitOfWork)
    {
        _workFieldRepository = workFieldRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ActionResponse<WorkFieldGetDto>> ExecuteAsync(WorkFieldCreateDto dto)
    {
        // Validar que la categoría existe
        var categoryValidation = await dto.WorkFieldCategoryId.ValidateWorkFieldCategoryExists(_categoryRepository);
        if (!categoryValidation.WasSuccessful)
            return ActionResponse<WorkFieldGetDto>.NotFound(categoryValidation.Message);

        // Validar duplicados
        var duplicateValidation = await dto.Name.ValidateDuplicateWorkField(_workFieldRepository);
        if (!duplicateValidation.WasSuccessful)
            return ActionResponse<WorkFieldGetDto>.Conflict(duplicateValidation.Message);

        // Crear entidad
        var workField = dto.ToEntity();

        // Guardar
        var createResult = await _workFieldRepository.AddAsync(workField);
        if (!createResult.WasSuccessful)
            return ActionResponse<WorkFieldGetDto>.Failure(
                "Error al crear el campo de trabajo.", ResultCode.DatabaseError);

        await _unitOfWork.CommitAsync();

        // Obtener con la categoría incluida
        var workFieldWithCategory = await _workFieldRepository.GetByIdWithCategoryAsync(workField.Id);
        if (workFieldWithCategory.WasSuccessful)
            return ActionResponse<WorkFieldGetDto>.Success(
                workFieldWithCategory.Result!.ToGetDto(),
                "Campo de trabajo creado exitosamente.");

        return ActionResponse<WorkFieldGetDto>.Success(
            workField.ToGetDto(),
            "Campo de trabajo creado exitosamente.");
    }
}
