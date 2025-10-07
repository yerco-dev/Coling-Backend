using Coling.Domain.Constants;
using Coling.Domain.Entities;
using Coling.Domain.Entities.UsersManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Coling.Infrastructure.Data.Seeders;

public class UsersSeeder
{
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;
    private readonly ILogger<UsersSeeder> _logger;

    public UsersSeeder(
        RoleManager<Role> roleManager,
        UserManager<User> userManager,
        AppDbContext context,
        ILogger<UsersSeeder> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedRolesAsync();
            await SeedUsersAsync();

            _logger.LogInformation("Seeding completado exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar el seeder.");
            throw;
        }
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleEntry in BusinessConstants.SystemRolesValues)
        {
            var roleName = roleEntry.Value;

            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                var role = new Role
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };

                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Rol '{RoleName}' creado exitosamente.", roleName);
                }
                else
                {
                    _logger.LogError("Error al crear el rol '{RoleName}': {Errors}",
                        roleName,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                _logger.LogInformation("El rol '{RoleName}' ya existe.", roleName);
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        await CreateUserIfNotExists(
            userName: "admin",
            email: "admin@coling.com",
            password: "Admin@123",
            firstName: "Administrador",
            maternalLastName: "Sistema",
            nationalId: "00000001",
            role: SystemRoles.Admin
        );

        await CreateUserIfNotExists(
            userName: "moderator",
            email: "moderator@coling.com",
            password: "Moderator@123",
            firstName: "Moderador",
            maternalLastName: "Sistema",
            nationalId: "00000002",
            role: SystemRoles.Moderator
        );
    }

    private async Task CreateUserIfNotExists(
        string userName,
        string email,
        string password,
        string firstName,
        string maternalLastName,
        string nationalId,
        SystemRoles role,
        string? paternalLastName = null)
    {
        var existingUser = await _userManager.FindByNameAsync(userName);

        if (existingUser != null)
        {
            _logger.LogInformation("Usuario '{UserName}' ya existe.", userName);
            return;
        }

        var person = new Person
        {
            NationalId = nationalId,
            FirstNames = firstName,
            PaternalLastName = paternalLastName,
            MaternalLastName = maternalLastName
        };

        _context.Persons.Add(person);
        await _context.SaveChangesAsync();

        var user = new User
        {
            UserName = userName,
            Email = email,
            PersonId = person.Id,
            EmailConfirmed = true
        };

        var createUserResult = await _userManager.CreateAsync(user, password);

        if (!createUserResult.Succeeded)
        {
            _logger.LogError("Error al crear usuario '{UserName}': {Errors}",
                userName,
                string.Join(", ", createUserResult.Errors.Select(e => e.Description)));
            return;
        }

        // Asignar rol
        var roleName = BusinessConstants.SystemRolesValues[role];
        var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);

        if (addToRoleResult.Succeeded)
        {
            _logger.LogInformation("Usuario '{UserName}' creado exitosamente con rol '{RoleName}'.", userName, roleName);
        }
        else
        {
            _logger.LogError("Error al asignar rol '{RoleName}' al usuario '{UserName}': {Errors}",
                roleName,
                userName,
                string.Join(", ", addToRoleResult.Errors.Select(e => e.Description)));
        }
    }
}
