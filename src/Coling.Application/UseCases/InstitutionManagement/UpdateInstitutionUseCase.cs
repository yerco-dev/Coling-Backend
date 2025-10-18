using Coling.Aplication.Validators;
using Coling.Application.DTOs.InstitutionManagement;
using Coling.Application.Mappers.ActionResponse;
using Coling.Application.Validators;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.InstitutionManagement;

public class UpdateInstitutionUseCase
{
    private readonly IInstitutionRepository _institutionRepository;
    private readonly IInstitutionTypeRepository _institutionTypeRepository;

    public UpdateInstitutionUseCase(
        IInstitutionRepository institutionRepository,
        IInstitutionTypeRepository institutionTypeRepository)
    {
        _institutionRepository = institutionRepository;
        _institutionTypeRepository = institutionTypeRepository;
    }

    public async Task<ActionResponse<InstitutionDetailDto>> ExecuteAsync(Guid institutionId, UpdateInstitutionDto dto)
    {
        // Validar DTO
        var dtoValidationResult = dto.TryValidateModel();
        if (!dtoValidationResult.WasSuccessful)
            return dtoValidationResult.ChangeNullActionResponseType<UpdateInstitutionDto, InstitutionDetailDto>();

        // Validar que el tipo de institución existe
        var institutionTypeValidation = await dto.InstitutionTypeId.ValidateInstitutionTypeExists(_institutionTypeRepository);
        if (!institutionTypeValidation.WasSuccessful)
            return institutionTypeValidation.ChangeNullActionResponseType<Guid, InstitutionDetailDto>();

        // Obtener institución
        var institutionResult = await _institutionRepository.GetAsync(institutionId);
        if (!institutionResult.WasSuccessful)
            return ActionResponse<InstitutionDetailDto>.NotFound("Institución no encontrada.");

        var institution = institutionResult.Result!;

        // Actualizar
        institution.Name = dto.Name;
        institution.InstitutionTypeId = dto.InstitutionTypeId;

        var updateResult = await _institutionRepository.UpdateAsync(institution);
        if (!updateResult.WasSuccessful)
            return updateResult.ChangeNullActionResponseType<Domain.Entities.InstitutionManagement.Institution, InstitutionDetailDto>();

        // Obtener tipo para respuesta
        var institutionTypeResult = await _institutionTypeRepository.GetAsync(institution.InstitutionTypeId);

        var detailDto = new InstitutionDetailDto
        {
            Id = institution.Id,
            Name = institution.Name,
            InstitutionTypeId = institution.InstitutionTypeId,
            InstitutionTypeName = institutionTypeResult.Result?.Name ?? "",
            IsActive = institution.IsActive
        };

        return ActionResponse<InstitutionDetailDto>.Success(
            detailDto,
            "Institución actualizada correctamente.");
    }
}
