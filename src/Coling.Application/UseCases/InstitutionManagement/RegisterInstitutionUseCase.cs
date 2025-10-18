using Coling.Aplication.Validators;
using Coling.Application.DTOs.InstitutionManagement;
using Coling.Application.Mappers.ActionResponse;
using Coling.Application.Mappers.InstitutionManagement;
using Coling.Application.Validators;
using Coling.Domain.Entities.InstitutionManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.InstitutionManagement;

public class RegisterInstitutionUseCase
{
    private readonly IInstitutionRepository _institutionRepository;
    private readonly IInstitutionTypeRepository _institutionTypeRepository;

    public RegisterInstitutionUseCase(
        IInstitutionRepository institutionRepository,
        IInstitutionTypeRepository institutionTypeRepository)
    {
        _institutionRepository = institutionRepository;
        _institutionTypeRepository = institutionTypeRepository;
    }

    public async Task<ActionResponse<InstitutionCreatedDto>> ExecuteAsync(RegisterInstitutionDto dto)
    {
        var dtoValidationResult = dto.TryValidateModel();
        if (!dtoValidationResult.WasSuccessful)
            return dtoValidationResult.ChangeNullActionResponseType<RegisterInstitutionDto, InstitutionCreatedDto>();

        var institutionTypeValidation = await dto.InstitutionTypeId.ValidateInstitutionTypeExists(_institutionTypeRepository);
        if (!institutionTypeValidation.WasSuccessful)
            return institutionTypeValidation.ChangeNullActionResponseType<Guid, InstitutionCreatedDto>();

        var duplicateValidation = await dto.Name.ValidateDuplicateInstitution(_institutionRepository);
        if (!duplicateValidation.WasSuccessful)
            return duplicateValidation.ChangeNullActionResponseType<Institution, InstitutionCreatedDto>();

        Institution institution = dto.ToInstitution();
        var modelValidation = institution.TryValidateModel();
        if (!modelValidation.WasSuccessful)
            return modelValidation.ChangeNullActionResponseType<Institution, InstitutionCreatedDto>();

        var createResult = await _institutionRepository.AddAsync(institution);
        if (!createResult.WasSuccessful)
            return createResult.ChangeNullActionResponseType<Institution, InstitutionCreatedDto>();

        var institutionType = await _institutionTypeRepository.GetAsync(dto.InstitutionTypeId);

        return ActionResponse<InstitutionCreatedDto>.Success(
            createResult.Result!.ToInstitutionCreatedDto(institutionType.Result?.Name ?? ""),
            "Institución registrada correctamente.");
    }
}
