using Coling.Application.DTOs.WorkManagement;
using Coling.Application.Mappers.WorkManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class GetAllWorkFieldCategoriesWithFieldsUseCase
{
    private readonly IWorkFieldCategoryRepository _repository;

    public GetAllWorkFieldCategoriesWithFieldsUseCase(IWorkFieldCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ActionResponse<IEnumerable<WorkFieldCategoryWithFieldsDto>>> ExecuteAsync()
    {
        var result = await _repository.GetAllWithWorkFieldsAsync();

        if (!result.WasSuccessful)
            return ActionResponse<IEnumerable<WorkFieldCategoryWithFieldsDto>>.Failure(
                "Error al obtener las categorías con campos de trabajo.", ResultCode.DatabaseError);

        var dtos = result.Result!.Select(c => c.ToWithFieldsDto());

        return ActionResponse<IEnumerable<WorkFieldCategoryWithFieldsDto>>.Success(dtos);
    }
}
