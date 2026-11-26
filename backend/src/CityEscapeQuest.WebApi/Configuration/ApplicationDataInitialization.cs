using CityEscapeQuest.Infrastructure.Identity;
using CityEscapeQuest.Infrastructure.Persistence;

namespace CityEscapeQuest.WebApi.Configuration;

internal static class ApplicationDataInitialization
{
    internal static async Task SeedApplicationDataAsync(this WebApplication app)
    {
        await IdentitySeeder.SeedRolesAsync(app.Services, app.Lifetime.ApplicationStopping);
        await MissionSeeder.SeedAsync(app.Services, app.Lifetime.ApplicationStopping);
    }
}
