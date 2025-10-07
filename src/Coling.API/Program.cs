using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Application.Interfaces.Services;
using Coling.Application.UseCases.MembersManagement;
using Coling.Application.UseCases.UsersManagement;
using Coling.Domain.Constants;
using Coling.Domain.Entities.UsersManagement;
using Coling.Domain.Interfaces.Repositories.MembersManagement;
using Coling.Domain.Interfaces.Repositories.PeopleManagement;
using Coling.Domain.Interfaces.Repositories.UsersManagement;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Data.Seeders;
using Coling.Infrastructure.Repositories.MembersManagement;
using Coling.Infrastructure.Repositories.PeopleManagement;
using Coling.Infrastructure.Repositories.UsersManagement;
using Coling.Infrastructure.Services;
using Coling.Infrastructure.UnitsOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(app =>
    {
        app.UseMiddleware<Coling.API.Middleware.JwtAuthenticationMiddleware>();
    })
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration["ConnectionStrings:DefaultConnection"];
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("ConnectionString 'DefaultConnection' no configurada.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentity<User, Role>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;

            options.User.RequireUniqueEmail = true;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole(BusinessConstants.SystemRolesValues[SystemRoles.Admin]));

            options.AddPolicy("AdminOrModerator", policy =>
                policy.RequireRole(
                    BusinessConstants.SystemRolesValues[SystemRoles.Admin],
                    BusinessConstants.SystemRolesValues[SystemRoles.Moderator]));

            options.AddPolicy("MemberOnly", policy =>
                policy.RequireRole(BusinessConstants.SystemRolesValues[SystemRoles.Member]));
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddScoped<IDbContextUnitOfWork, DbContextUnitOfWork>();

        services.AddScoped<ITokenService, JwtService>();




        services.AddScoped<RegisterMemberUserUseCase>();
        services.AddScoped<LoginUseCase>();

        services.AddScoped<GetPendingMembersUseCase>();
        services.AddScoped<ApproveMemberUseCase>();
        services.AddScoped<RejectMemberUseCase>();

        services.AddScoped<UsersSeeder>();

        services.AddApplicationInsightsTelemetryWorkerService();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var seeder = services.GetRequiredService<UsersSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al ejecutar el seeder de roles.");
    }
}

host.Run();
