using Coling.Application.DTOs.WorkManagement;
using Coling.Application.Mappers.WorkManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.WorkManagement;

public class GetWorkFieldByIdUseCase
{
    private readonly IWorkFieldRepository _repository;

    public GetWorkFieldByIdUseCase(IWorkFieldRepository repository)
    {
        _repository = repository;
    }

    public async Task<ActionResponse<WorkFieldGetDto>> ExecuteAsync(Guid id)
    {
        var result = await _repository.GetByIdWithCategoryAsync(id);

        if (!result.WasSuccessful)
            return ActionResponse<WorkFieldGetDto>.NotFound("Campo de trabajo no encontrado.");

        return ActionResponse<WorkFieldGetDto>.Success(result.Result!.ToGetDto());
    }
}
