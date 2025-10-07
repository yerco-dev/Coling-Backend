using Coling.Domain.Entities.ActionResponse;
using Coling.Domain.Entities.UsersManagement;
using Coling.Domain.Interfaces.Repositories.Generics;

namespace Coling.Domain.Interfaces.Repositories.UsersManagement;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<ActionResponse<bool>> RoleExist(string role);
}