using Coling.Application.DTOs.InstitutionManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.InstitutionManagement;

public class GetInstitutionTypeByIdUseCase
{
    private readonly IInstitutionTypeRepository _institutionTypeRepository;

    public GetInstitutionTypeByIdUseCase(IInstitutionTypeRepository institutionTypeRepository)
    {
        _institutionTypeRepository = institutionTypeRepository;
    }

    public async Task<ActionResponse<InstitutionTypeDetailDto>> ExecuteAsync(Guid institutionTypeId)
    {
        var result = await _institutionTypeRepository.GetAsync(institutionTypeId);

        if (!result.WasSuccessful)
            return ActionResponse<InstitutionTypeDetailDto>.NotFound("Tipo de institución no encontrado.");

        var institutionType = result.Result!;

        var detailDto = new InstitutionTypeDetailDto
        {
            Id = institutionType.Id,
            Name = institutionType.Name,
            IsActive = institutionType.IsActive
        };

        return ActionResponse<InstitutionTypeDetailDto>.Success(
            detailDto,
            "Tipo de institución obtenido correctamente.");
    }
}
