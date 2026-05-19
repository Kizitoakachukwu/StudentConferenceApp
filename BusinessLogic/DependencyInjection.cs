using Microsoft.Extensions.DependencyInjection;
using StudentConferenceApp.BLL.Interfaces;
using StudentConferenceApp.BLL.Services;

namespace StudentConferenceApp.BLL;

/// <summary>Registers application / business services (business logic layer).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        services.AddScoped<IParticipantService, ParticipantService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<ISectionService, SectionService>();

        return services;
    }
}
