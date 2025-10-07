using Coling.Domain.Entities.ActionResponse;
using Coling.Domain.Entities.UsersManagement;
using Coling.Domain.Interfaces.Repositories.Generics;
using Coling.Domain.Interfaces.Repositories.UsersManagement;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Repositories.Generics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Coling.Infrastructure.Repositories.UsersManagement;
public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly SignInManager<User> _signInManager;

    public UserRepository(AppDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager) : base(context)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }
    public override async Task<ActionResponse<IEnumerable<User>>> GetAsync(bool includeDeleteds = false)
    {
        var user = _context.UserRoles.FirstOrDefault();

        return new ActionResponse<IEnumerable<User>>
        {
            WasSuccessful = true,
            Result = await _context.Users
            .Include(u => u.Person)
            .Where(u => u.IsActive || includeDeleteds)
            .ToListAsync(),
        };
    }


    public async Task<ActionResponse<User>> AssignRole(User user, string role)
    {
        try
        {
            var result = await _userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                return new ActionResponse<User>
                {
                    WasSuccessful = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description)),
                    ResultCode = ResultCode.DatabaseError
                };
            }

            return new ActionResponse<User>
            {
                WasSuccessful = true,
                Message = $"Rol {role} asignado correctamente.",
                ResultCode = ResultCode.Ok,
                Result = user
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<User>
            {
                WasSuccessful = false,
                Message = ex.Message,
                ResultCode = ResultCode.DatabaseError
            };
        }
    }

    public async Task<ActionResponse<User>> FindByName(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);



        if (user == null)
        {
            return new ActionResponse<User>
            {
                WasSuccessful = false,
                Message = "Usuario no encontrado.",
                ResultCode = ResultCode.NotFound
            };
        }
        return new ActionResponse<User>
        {
            WasSuccessful = true,
            ResultCode = ResultCode.NotFound,
            Result = user
        };
    }

    public async Task<ActionResponse<User>> GetFullData(Guid id)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return new ActionResponse<User>
                {
                    WasSuccessful = false,
                    Message = "Usuario no encontrado.",
                    ResultCode = ResultCode.NotFound
                };
            }

            return new ActionResponse<User>
            {
                WasSuccessful = true,
                Message = "Usuario encontrado.",
                ResultCode = ResultCode.Ok,
                Result = user
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<User>
            {
                WasSuccessful = false,
                Message = ex.Message,
                ResultCode = ResultCode.DatabaseError
            };
        }
    }

    public async Task<ActionResponse<SignInResult>> LoginAsync(string userName, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(userName, password, false, false);
        if (result.Succeeded)
            return new ActionResponse<SignInResult>
            {
                WasSuccessful = true,
                Result = result,
                ResultCode = ResultCode.Ok
            };

        return new ActionResponse<SignInResult>
        {
            WasSuccessful = false,
            Message = "Fallo al iniciar sesión.",
            Result = result,
        };
    }

    public async Task<ActionResponse<bool>> UserHasRole(User user, string role)
    {

        if (await _userManager.IsInRoleAsync(user, role))
        {
            return new ActionResponse<bool>
            {
                WasSuccessful = true,
                ResultCode = ResultCode.Ok,
                Result = true
            };
        }
        return new ActionResponse<bool>
        {
            WasSuccessful = true,
            ResultCode = ResultCode.Ok,
            Result = false
        };
    }

    public async Task<ActionResponse<string>> GetRole(User user)
    {
        var roles = await GetRolesAsync(user);

        return new ActionResponse<string>
        {
            Result = roles.Count > 0 ? roles[0] : "",
        };
    }

    public async Task<ActionResponse<bool>> SetPassword(User user, string password)
    {
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, resetToken, password);

        if (!resetPasswordResult.Succeeded)
            return new ActionResponse<bool>
            {
                WasSuccessful = false,
                Result = false,
                ResultCode = ResultCode.DatabaseError,
                Message = $"Error resetando el password del usuario {user.UserName}: {string.Join(", ", resetPasswordResult.Errors.Select(e => e.Description))}",
            };

        return new ActionResponse<bool>
        {
            Result = true,
        };
    }

    public async Task<ActionResponse<bool>> VerifyUser(User user, string hashPassword)
    {
        try
        {
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, hashPassword);

            return new ActionResponse<bool>
            {
                WasSuccessful = isPasswordValid,
                Message = isPasswordValid ? "Credenciales válidas." : "Contraseña incorrecta.",
                ResultCode = isPasswordValid ? ResultCode.Ok : ResultCode.NotFound,
                Result = isPasswordValid
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<bool>
            {
                WasSuccessful = false,
                Message = ex.Message,
                ResultCode = ResultCode.DatabaseError,
                Result = false
            };
        }
    }
    public async Task<List<string>> GetRolesAsync(User user)
    {
        if (user == null) return new List<string>();

        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    public async Task<ActionResponse<User>> AddAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return new ActionResponse<User>
            {
                WasSuccessful = false,
                Message = $"Error creando usuario {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}",
                ResultCode = ResultCode.DatabaseError

            };
        }

        return new ActionResponse<User>
        {
            WasSuccessful = true,
            Result = user,
            ResultCode = ResultCode.Ok
        };
    }

    public async Task<ActionResponse<bool>> RerolUser(User user, string newRol)
    {
        var userRol = await GetRole(user);

        if (!userRol.WasSuccessful)
            return new ActionResponse<bool>
            {
                WasSuccessful = false,
                Message = $"Error al obtener el rol actual del usuario",
                ResultCode = ResultCode.DatabaseError

            };

        try
        {
            var removeRolResult = await _userManager.RemoveFromRoleAsync(user, userRol.Result!);

            if (!removeRolResult.Succeeded)
            {
                return new ActionResponse<bool>
                {
                    WasSuccessful = false,
                    Message = $"Error al quitar el rol al usuario {user.UserName}: {string.Join(", ", removeRolResult.Errors.Select(e => e.Description))}",
                    ResultCode = ResultCode.DatabaseError

                };
            }

            var asignRoleReslt = await AssignRole(user, newRol);


            if (!removeRolResult.Succeeded)
                return new ActionResponse<bool>
                {
                    WasSuccessful = false,
                    Message = $"Error al asignar el rol al usuario {user.UserName}",
                    ResultCode = ResultCode.DatabaseError

                };

            return new ActionResponse<bool>
            {
                Result = true
            };

        }
        catch (Exception ex)
        {
            return new ActionResponse<bool>
            {
                WasSuccessful = false,
                Message = $"Error inesperado {ex.Message}",
                ResultCode = ResultCode.DatabaseError,
                Result = false
            };
        }
    }
}
