using Coling.Domain.Entities.InstitutionManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Wrappers;

namespace Coling.Application.Validators;

public static class InstitutionValidator
{
    public static async Task<ActionResponse<Guid>> ValidateInstitutionTypeExists(
        this Guid institutionTypeId, IInstitutionTypeRepository repository)
    {
        var institutionType = await repository.GetAsync(institutionTypeId);

        if (!institutionType.WasSuccessful)
            return ActionResponse<Guid>.NotFound("El tipo de institución no existe.");

        return ActionResponse<Guid>.Success(institutionTypeId);
    }

    public static async Task<ActionResponse<Institution>> ValidateDuplicateInstitution(
        this string institutionName, IInstitutionRepository repository)
    {
        var existing = await repository.GetAsync(i =>
            i.Name.ToLower() == institutionName.ToLower() && i.IsActive);

        if (existing.WasSuccessful)
            return ActionResponse<Institution>.Conflict("Ya existe una institución con ese nombre.");

        return ActionResponse<Institution>.Success(null!);
    }

    public static async Task<ActionResponse<InstitutionType>> ValidateDuplicateInstitutionType(
        this string institutionTypeName, IInstitutionTypeRepository repository)
    {
        var existing = await repository.GetAsync(it =>
            it.Name.ToLower() == institutionTypeName.ToLower() && it.IsActive);

        if (existing.WasSuccessful)
            return ActionResponse<InstitutionType>.Conflict("Ya existe un tipo de institución con ese nombre.");

        return ActionResponse<InstitutionType>.Success(null!);
    }
}
