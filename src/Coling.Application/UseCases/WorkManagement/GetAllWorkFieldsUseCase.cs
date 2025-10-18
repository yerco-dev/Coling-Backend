using Coling.Application.DTOs.WorkManagement;
using Coling.Application.Mappers.WorkManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class GetAllWorkFieldsUseCase
{
    private readonly IWorkFieldRepository _repository;

    public GetAllWorkFieldsUseCase(IWorkFieldRepository repository)
    {
        _repository = repository;
    }

    public async Task<ActionResponse<IEnumerable<WorkFieldGetDto>>> ExecuteAsync()
    {
        var result = await _repository.GetAsync(includeDeleteds: false);

        if (!result.WasSuccessful)
            return ActionResponse<IEnumerable<WorkFieldGetDto>>.Failure(
                "Error al obtener los campos de trabajo.", ResultCode.DatabaseError);

        // Cargar categorías para cada work field
        var workFieldsWithCategories = new List<WorkFieldGetDto>();
        foreach (var wf in result.Result!)
        {
            var wfWithCategory = await _repository.GetByIdWithCategoryAsync(wf.Id);
            if (wfWithCategory.WasSuccessful)
                workFieldsWithCategories.Add(wfWithCategory.Result!.ToGetDto());
        }

        return ActionResponse<IEnumerable<WorkFieldGetDto>>.Success(workFieldsWithCategories);
    }
}
