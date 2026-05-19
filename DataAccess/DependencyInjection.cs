using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentConferenceApp.DAL.Repositories;
using StudentConferenceApp.DAL.Repositories.Interfaces;

namespace StudentConferenceApp.DataAccess;

/// <summary>Registers EF Core <see cref="ApplicationDbContext"/> and repository implementations (data access layer).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IParticipantRepository, ParticipantRepository>();
        services.AddScoped<IManagerRepository, ManagerRepository>();

        return services;
    }
}
