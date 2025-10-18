using Coling.Application.DTOs.WorkManagement;
using Coling.Application.Mappers.WorkManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class GetWorkFieldCategoryByIdUseCase
{
    private readonly IWorkFieldCategoryRepository _repository;

    public GetWorkFieldCategoryByIdUseCase(IWorkFieldCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ActionResponse<WorkFieldCategoryWithFieldsDto>> ExecuteAsync(Guid id)
    {
        var result = await _repository.GetByIdWithWorkFieldsAsync(id);

        if (!result.WasSuccessful)
            return ActionResponse<WorkFieldCategoryWithFieldsDto>.NotFound(
                "Categoría de campo de trabajo no encontrada.");

        return ActionResponse<WorkFieldCategoryWithFieldsDto>.Success(
            result.Result!.ToWithFieldsDto());
    }
}
