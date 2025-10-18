using Coling.Aplication.Validators;
using Coling.Application.DTOs.InstitutionManagement;
using Coling.Application.Mappers.ActionResponse;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.InstitutionManagement;

public class UpdateInstitutionTypeUseCase
{
    private readonly IInstitutionTypeRepository _institutionTypeRepository;

    public UpdateInstitutionTypeUseCase(IInstitutionTypeRepository institutionTypeRepository)
    {
        _institutionTypeRepository = institutionTypeRepository;
    }

    public async Task<ActionResponse<InstitutionTypeDetailDto>> ExecuteAsync(Guid institutionTypeId, UpdateInstitutionTypeDto dto)
    {
        // Validar DTO
        var dtoValidationResult = dto.TryValidateModel();
        if (!dtoValidationResult.WasSuccessful)
            return dtoValidationResult.ChangeNullActionResponseType<UpdateInstitutionTypeDto, InstitutionTypeDetailDto>();

        // Obtener tipo de institución
        var institutionTypeResult = await _institutionTypeRepository.GetAsync(institutionTypeId);
        if (!institutionTypeResult.WasSuccessful)
            return ActionResponse<InstitutionTypeDetailDto>.NotFound("Tipo de institución no encontrado.");

        var institutionType = institutionTypeResult.Result!;

        // Actualizar
        institutionType.Name = dto.Name;

        var updateResult = await _institutionTypeRepository.UpdateAsync(institutionType);
        if (!updateResult.WasSuccessful)
            return updateResult.ChangeNullActionResponseType<Domain.Entities.InstitutionManagement.InstitutionType, InstitutionTypeDetailDto>();

        var detailDto = new InstitutionTypeDetailDto
        {
            Id = institutionType.Id,
            Name = institutionType.Name,
            IsActive = institutionType.IsActive
        };

        return ActionResponse<InstitutionTypeDetailDto>.Success(
            detailDto,
            "Tipo de institución actualizado correctamente.");
    }
}
