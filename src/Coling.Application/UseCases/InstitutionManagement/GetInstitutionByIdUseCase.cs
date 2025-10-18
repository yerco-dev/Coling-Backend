using Coling.Application.DTOs.InstitutionManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.InstitutionManagement;

public class GetInstitutionByIdUseCase
{
    private readonly IInstitutionRepository _institutionRepository;
    private readonly IInstitutionTypeRepository _institutionTypeRepository;

    public GetInstitutionByIdUseCase(
        IInstitutionRepository institutionRepository,
        IInstitutionTypeRepository institutionTypeRepository)
    {
        _institutionRepository = institutionRepository;
        _institutionTypeRepository = institutionTypeRepository;
    }

    public async Task<ActionResponse<InstitutionDetailDto>> ExecuteAsync(Guid institutionId)
    {
        var institutionResult = await _institutionRepository.GetAsync(institutionId);

        if (!institutionResult.WasSuccessful)
            return ActionResponse<InstitutionDetailDto>.NotFound("Institución no encontrada.");

        var institution = institutionResult.Result!;

        var institutionTypeResult = await _institutionTypeRepository.GetAsync(institution.InstitutionTypeId);
        var institutionTypeName = institutionTypeResult.Result?.Name ?? "";

        var detailDto = new InstitutionDetailDto
        {
            Id = institution.Id,
            Name = institution.Name,
            InstitutionTypeId = institution.InstitutionTypeId,
            InstitutionTypeName = institutionTypeName,
            IsActive = institution.IsActive
        };

        return ActionResponse<InstitutionDetailDto>.Success(
            detailDto,
            "Institución obtenida correctamente.");
    }
}
