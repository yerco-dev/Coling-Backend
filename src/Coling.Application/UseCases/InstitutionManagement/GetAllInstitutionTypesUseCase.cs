using Coling.Application.DTOs.InstitutionManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.InstitutionManagement;

public class GetAllInstitutionTypesUseCase
{
    private readonly IInstitutionTypeRepository _institutionTypeRepository;

    public GetAllInstitutionTypesUseCase(IInstitutionTypeRepository institutionTypeRepository)
    {
        _institutionTypeRepository = institutionTypeRepository;
    }

    public async Task<ActionResponse<IEnumerable<InstitutionTypeDetailDto>>> ExecuteAsync()
    {
        var result = await _institutionTypeRepository.GetAsync();

        if (!result.WasSuccessful)
            return ActionResponse<IEnumerable<InstitutionTypeDetailDto>>.Failure(
                result.Message ?? "Error al obtener los tipos de institución.",
                ResultCode.DatabaseError);

        var detailList = result.Result?.Select(it => new InstitutionTypeDetailDto
        {
            Id = it.Id,
            Name = it.Name,
            IsActive = it.IsActive
        }).ToList() ?? new List<InstitutionTypeDetailDto>();

        return ActionResponse<IEnumerable<InstitutionTypeDetailDto>>.Success(
            detailList,
            "Tipos de institución obtenidos correctamente.");
    }
}
