using Coling.Domain.Wrappers;
using Coling.Domain.Entities.UsersManagement;
using Coling.Domain.Interfaces.Repositories.UsersManagement;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Repositories.Generics;
using Microsoft.AspNetCore.Identity;

namespace Coling.Infrastructure.Repositories.UsersManagement;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    private readonly RoleManager<Role> _roleManager;

    public RoleRepository(AppDbContext context, RoleManager<Role> roleManager) : base(context)
    {
        _roleManager = roleManager;
    }
    public async Task<ActionResponse<bool>> RoleExist(string role)
    {
        if (!await _roleManager.RoleExistsAsync(role))
        {
            return new ActionResponse<bool>
            {
                WasSuccessful = false,
                Message = $"El rol {role} no existe.",
                Result = false
            };
        }

        return new ActionResponse<bool>
        {
            Result = true
        };
    }
}