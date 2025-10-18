using Coling.Application.DTOs.WorkManagement;
using Coling.Application.Mappers.WorkManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class GetAllWorkFieldCategoriesUseCase
{
    private readonly IWorkFieldCategoryRepository _repository;

    public GetAllWorkFieldCategoriesUseCase(IWorkFieldCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ActionResponse<IEnumerable<WorkFieldCategoryGetDto>>> ExecuteAsync()
    {
        var result = await _repository.GetAsync(includeDeleteds: false);

        if (!result.WasSuccessful)
            return ActionResponse<IEnumerable<WorkFieldCategoryGetDto>>.Failure(
                "Error al obtener las categorías de campos de trabajo.", ResultCode.DatabaseError);

        var dtos = result.Result!.Select(c => c.ToGetDto());

        return ActionResponse<IEnumerable<WorkFieldCategoryGetDto>>.Success(dtos);
    }
}
