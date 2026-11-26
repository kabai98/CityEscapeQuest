using CityEscapeQuest.Application.GameSessions;
using CityEscapeQuest.Application.Missions;
using Microsoft.Extensions.DependencyInjection;

namespace CityEscapeQuest.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<MissionCatalogService>();
        services.AddScoped<GameSessionLobbyService>();
        services.AddScoped<GameSessionPlayService>();
        services.AddSingleton(TimeProvider.System);
        return services;
    }
}