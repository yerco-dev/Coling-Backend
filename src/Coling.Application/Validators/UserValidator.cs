using Coling.Domain.Wrappers;
using Coling.Domain.Entities.UsersManagement;
using Coling.Domain.Interfaces.Repositories.UsersManagement;

namespace Coling.Aplication.Validators;

public static class UserValidator
{
    public static async Task<ActionResponse<User>> ValidateUniqueUserName(this string UserName, IUserRepository repository, Guid? omitedGuid = null)
    {
        var existingUser = await repository.GetAsync(u => u.UserName == UserName);

        if (existingUser.WasSuccessful && (omitedGuid == null || existingUser.Result!.Id != omitedGuid))
            return ActionResponse<User>.Conflict("El UserName ya se encuentra registrado en el sistema.");

        return ActionResponse<User>.Success(existingUser.Result);
    }
}
