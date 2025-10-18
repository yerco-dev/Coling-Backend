using Coling.Application.DTOs.WorkManagement;
using Coling.Application.Mappers.WorkManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class GetWorkFieldsByCategoryUseCase
{
    private readonly IWorkFieldRepository _repository;

    public GetWorkFieldsByCategoryUseCase(IWorkFieldRepository repository)
    {
        _repository = repository;
    }

    public async Task<ActionResponse<IEnumerable<WorkFieldGetDto>>> ExecuteAsync(Guid categoryId)
    {
        var result = await _repository.GetByCategoryIdAsync(categoryId);

        if (!result.WasSuccessful)
            return ActionResponse<IEnumerable<WorkFieldGetDto>>.Failure(
                "Error al obtener los campos de trabajo de la categoría.", ResultCode.DatabaseError);

        var dtos = result.Result!.Select(wf => wf.ToGetDto());

        return ActionResponse<IEnumerable<WorkFieldGetDto>>.Success(dtos);
    }
}
