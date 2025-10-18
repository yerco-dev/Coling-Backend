using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Coling.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        var apiProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Coling.API");

        IConfigurationRoot configuration;

        if (Directory.Exists(apiProjectPath))
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("local.settings.json", optional: true)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
        }
        else
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
        }

        var connectionString = configuration["ConnectionStrings:DefaultConnection"];

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionString 'DefaultConnection' no encontrado. " +
                "Asegúrate de que local.settings.json o appsettings.json exista en el proyecto Coling.API con la configuración correcta.");
        }

        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
