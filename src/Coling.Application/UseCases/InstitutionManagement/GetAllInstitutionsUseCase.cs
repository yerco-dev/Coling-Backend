using Coling.Application.DTOs.InstitutionManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.InstitutionManagement;

public class GetAllInstitutionsUseCase
{
    private readonly IInstitutionRepository _institutionRepository;
    private readonly IInstitutionTypeRepository _institutionTypeRepository;

    public GetAllInstitutionsUseCase(
        IInstitutionRepository institutionRepository,
        IInstitutionTypeRepository institutionTypeRepository)
    {
        _institutionRepository = institutionRepository;
        _institutionTypeRepository = institutionTypeRepository;
    }

    public async Task<ActionResponse<IEnumerable<InstitutionDetailDto>>> ExecuteAsync()
    {
        var institutionsResult = await _institutionRepository.GetAsync();

        if (!institutionsResult.WasSuccessful)
            return ActionResponse<IEnumerable<InstitutionDetailDto>>.Failure(
                institutionsResult.Message ?? "Error al obtener las instituciones.",
                ResultCode.DatabaseError);

        var detailList = new List<InstitutionDetailDto>();

        foreach (var institution in institutionsResult.Result ?? Enumerable.Empty<Domain.Entities.InstitutionManagement.Institution>())
        {
            var institutionTypeResult = await _institutionTypeRepository.GetAsync(institution.InstitutionTypeId);
            var institutionTypeName = institutionTypeResult.Result?.Name ?? "";

            detailList.Add(new InstitutionDetailDto
            {
                Id = institution.Id,
                Name = institution.Name,
                InstitutionTypeId = institution.InstitutionTypeId,
                InstitutionTypeName = institutionTypeName,
                IsActive = institution.IsActive
            });
        }

        return ActionResponse<IEnumerable<InstitutionDetailDto>>.Success(
            detailList,
            "Instituciones obtenidas correctamente.");
    }
}
