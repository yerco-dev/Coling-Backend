using Coling.Aplication.Validators;
using Coling.Application.DTOs.InstitutionManagement;
using Coling.Application.Mappers.ActionResponse;
using Coling.Application.Mappers.InstitutionManagement;
using Coling.Application.Validators;
using Coling.Domain.Entities.InstitutionManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.UseCases.InstitutionManagement;

public class RegisterInstitutionTypeUseCase
{
    private readonly IInstitutionTypeRepository _institutionTypeRepository;

    public RegisterInstitutionTypeUseCase(IInstitutionTypeRepository institutionTypeRepository)
    {
        _institutionTypeRepository = institutionTypeRepository;
    }

    public async Task<ActionResponse<InstitutionTypeCreatedDto>> ExecuteAsync(RegisterInstitutionTypeDto dto)
    {
        var dtoValidationResult = dto.TryValidateModel();
        if (!dtoValidationResult.WasSuccessful)
            return dtoValidationResult.ChangeNullActionResponseType<RegisterInstitutionTypeDto, InstitutionTypeCreatedDto>();

        var duplicateValidation = await dto.Name.ValidateDuplicateInstitutionType(_institutionTypeRepository);
        if (!duplicateValidation.WasSuccessful)
            return duplicateValidation.ChangeNullActionResponseType<InstitutionType, InstitutionTypeCreatedDto>();

        InstitutionType institutionType = dto.ToInstitutionType();
        var modelValidation = institutionType.TryValidateModel();
        if (!modelValidation.WasSuccessful)
            return modelValidation.ChangeNullActionResponseType<InstitutionType, InstitutionTypeCreatedDto>();

        var createResult = await _institutionTypeRepository.AddAsync(institutionType);
        if (!createResult.WasSuccessful)
            return createResult.ChangeNullActionResponseType<InstitutionType, InstitutionTypeCreatedDto>();

        return ActionResponse<InstitutionTypeCreatedDto>.Success(
            createResult.Result!.ToInstitutionTypeCreatedDto(),
            "Tipo de institución registrado correctamente.");
    }
}
