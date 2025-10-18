using Coling.Aplication.Interfaces.UnitsOfWork;
using Coling.Application.Interfaces.Services;
using Coling.Application.Interfaces.Services.Storage;
using Coling.Application.UseCases.AcademicManagement;
using Coling.Application.UseCases.InstitutionManagement;
using Coling.Application.UseCases.MembersManagement;
using Coling.Application.UseCases.UsersManagement;
using Coling.Application.UseCases.WorkManagement;
using Coling.Domain.Constants;
using Coling.Domain.Entities.UsersManagement;
using Coling.Domain.Interfaces.Repositories.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Domain.Interfaces.Repositories.MembersManagement;
using Coling.Domain.Interfaces.Repositories.PeopleManagement;
using Coling.Domain.Interfaces.Repositories.UsersManagement;
using Coling.Domain.Interfaces.Repositories.WorkManagement;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Data.Seeders;
using Coling.Infrastructure.Repositories.AcademicManagement;
using Coling.Infrastructure.Repositories.InstitutionManagement;
using Coling.Infrastructure.Repositories.MembersManagement;
using Coling.Infrastructure.Repositories.PeopleManagement;
using Coling.Infrastructure.Repositories.UsersManagement;
using Coling.Infrastructure.Repositories.WorkManagement;
using Coling.Infrastructure.Services;
using Coling.Infrastructure.Services.Storage;
using Coling.Infrastructure.UnitsOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;

// Configurar InvariantCulture para toda la aplicación
// Esto asegura que los números usen punto decimal (.) en lugar de coma (,)
// Estándar para APIs REST que deben ser independientes de la cultura del servidor
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

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

        // Academic Management Repositories
        services.AddScoped<IEducationRepository, EducationRepository>();
        services.AddScoped<IDegreeEducationRepository, DegreeEducationRepository>();
        services.AddScoped<IContinuingEducationRepository, ContinuingEducationRepository>();
        services.AddScoped<IProfessionalCertificationRepository, ProfessionalCertificationRepository>();
        services.AddScoped<IMemberEducationRepository, MemberEducationRepository>();

        // Institution Management Repositories
        services.AddScoped<IInstitutionRepository, InstitutionRepository>();
        services.AddScoped<IInstitutionTypeRepository, InstitutionTypeRepository>();

        // Work Management Repositories
        services.AddScoped<IWorkFieldCategoryRepository, WorkFieldCategoryRepository>();
        services.AddScoped<IWorkFieldRepository, WorkFieldRepository>();
        services.AddScoped<IWorkExperienceRepository, WorkExperienceRepository>();
        services.AddScoped<IWorkExperienceFieldRepository, WorkExperienceFieldRepository>();


        // Services
        services.AddScoped<IDbContextUnitOfWork, DbContextUnitOfWork>();

        services.AddScoped<ITokenService, JwtService>();

        // Configurar Azure Blob Storage con lazy initialization
        services.AddScoped<IBlobStorageService, BlobStorageService>();




        // Users Management Use Cases
        services.AddScoped<RegisterMemberUserUseCase>();
        services.AddScoped<LoginUseCase>();

        // Members Management Use Cases
        services.AddScoped<GetPendingMembersUseCase>();
        services.AddScoped<ApproveMemberUseCase>();
        services.AddScoped<RejectMemberUseCase>();

        // Academic Management Use Cases
        services.AddScoped<RegisterDegreeEducationUseCase>();
        services.AddScoped<GetMyDegreeEducationsUseCase>();
        services.AddScoped<UpdateDegreeEducationUseCase>();
        services.AddScoped<DeleteDegreeEducationUseCase>();

        services.AddScoped<RegisterContinuingEducationUseCase>();
        services.AddScoped<GetMyContinuingEducationsUseCase>();
        services.AddScoped<UpdateContinuingEducationUseCase>();
        services.AddScoped<DeleteContinuingEducationUseCase>();

        services.AddScoped<RegisterProfessionalCertificationUseCase>();
        services.AddScoped<GetMyProfessionalCertificationsUseCase>();
        services.AddScoped<UpdateProfessionalCertificationUseCase>();
        services.AddScoped<DeleteProfessionalCertificationUseCase>();

        // Institution Management Use Cases
        services.AddScoped<RegisterInstitutionUseCase>();
        services.AddScoped<GetAllInstitutionsUseCase>();
        services.AddScoped<GetInstitutionByIdUseCase>();
        services.AddScoped<UpdateInstitutionUseCase>();
        services.AddScoped<DeleteInstitutionUseCase>();

        services.AddScoped<RegisterInstitutionTypeUseCase>();
        services.AddScoped<GetAllInstitutionTypesUseCase>();
        services.AddScoped<GetInstitutionTypeByIdUseCase>();
        services.AddScoped<UpdateInstitutionTypeUseCase>();
        services.AddScoped<DeleteInstitutionTypeUseCase>();

        // Work Management Use Cases
        services.AddScoped<CreateWorkFieldCategoryUseCase>();
        services.AddScoped<GetAllWorkFieldCategoriesUseCase>();
        services.AddScoped<GetAllWorkFieldCategoriesWithFieldsUseCase>();
        services.AddScoped<GetWorkFieldCategoryByIdUseCase>();
        services.AddScoped<UpdateWorkFieldCategoryUseCase>();
        services.AddScoped<DeleteWorkFieldCategoryUseCase>();

        services.AddScoped<CreateWorkFieldUseCase>();
        services.AddScoped<GetAllWorkFieldsUseCase>();
        services.AddScoped<GetWorkFieldByIdUseCase>();
        services.AddScoped<GetWorkFieldsByCategoryUseCase>();
        services.AddScoped<UpdateWorkFieldUseCase>();
        services.AddScoped<DeleteWorkFieldUseCase>();

        services.AddScoped<RegisterWorkExperienceUseCase>();
        services.AddScoped<GetMyWorkExperiencesUseCase>();
        services.AddScoped<GetWorkExperienceByIdUseCase>();
        services.AddScoped<UpdateWorkExperienceUseCase>();
        services.AddScoped<DeleteWorkExperienceUseCase>();

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
