using Coling.Domain.Entities;
using Coling.Domain.Wrappers;
using Coling.Domain.Interfaces.Repositories.PeopleManagement;

namespace Coling.Application.Validators;

public static class PersonValidator
{
    public static async Task<ActionResponse<Person>> ValidateUniqueNationalId(this string nationalId, IPersonRepository repository, Guid? omitedGuid = null)
    {
        var existingPerson = await repository.GetAsync(p => p.NationalId == nationalId);

        if (existingPerson.WasSuccessful && (omitedGuid == null || existingPerson.Result!.Id != omitedGuid))
            return ActionResponse<Person>.Conflict("El número de identificación ya se encuentra registrado en el sistema.");

        return ActionResponse<Person>.Success(existingPerson.Result);
    }
}
