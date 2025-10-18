using Coling.Application.DTOs.WorkManagement;
using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Application.Mappers.WorkManagement;
using Coling.Application.Validators;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class CreateWorkFieldCategoryUseCase
{
    private readonly IWorkFieldCategoryRepository _repository;
    private readonly IDbContextUnitOfWork _unitOfWork;

    public CreateWorkFieldCategoryUseCase(
        IWorkFieldCategoryRepository repository,
        IDbContextUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ActionResponse<WorkFieldCategoryGetDto>> ExecuteAsync(WorkFieldCategoryCreateDto dto)
    {
        // Validar duplicados
        var duplicateValidation = await dto.Name.ValidateDuplicateWorkFieldCategory(_repository);
        if (!duplicateValidation.WasSuccessful)
            return ActionResponse<WorkFieldCategoryGetDto>.Conflict(duplicateValidation.Message);

        // Crear entidad
        var category = dto.ToEntity();

        // Guardar
        var createResult = await _repository.AddAsync(category);
        if (!createResult.WasSuccessful)
            return ActionResponse<WorkFieldCategoryGetDto>.Failure(
                "Error al crear la categoría de campo de trabajo.", ResultCode.DatabaseError);

        await _unitOfWork.CommitAsync();

        return ActionResponse<WorkFieldCategoryGetDto>.Success(
            category.ToGetDto(),
            "Categoría de campo de trabajo creada exitosamente.");
    }
}
