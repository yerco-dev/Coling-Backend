using Coling.Domain.Entities.ActionResponse;
using Coling.Domain.Entities.UsersManagement;
using Coling.Domain.Interfaces.Repositories.Generics;
using Microsoft.AspNetCore.Identity;

namespace Coling.Domain.Interfaces.Repositories.UsersManagement;

public interface IUserRepository : IGenericRepository<User>
{
    Task<ActionResponse<User>> AddAsync(User user, string password);
    Task<ActionResponse<User>> FindByName(string userName);
    Task<ActionResponse<User>> AssignRole(User user, string role);
    Task<ActionResponse<User>> GetFullData(Guid id);
    Task<ActionResponse<bool>> VerifyUser(User user, string hashPassword);
    Task<ActionResponse<bool>> UserHasRole(User user, string role);
    Task<ActionResponse<SignInResult>> LoginAsync(string userName, string password);
    Task<List<string>> GetRolesAsync(User user);
    Task<ActionResponse<string>> GetRole(User user);
    Task<ActionResponse<bool>> SetPassword(User user, string password);
    Task<ActionResponse<bool>> RerolUser(User user, string newRol);

}

