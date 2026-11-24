using CityEscapeQuest.Domain.Missions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CityEscapeQuest.Infrastructure.Persistence;

public static class MissionSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (await dbContext.Missions.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.Missions.AddRange(
            new Mission(
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "A budai kód",
                "Egy eltűnt térképész nyomában a Várnegyed sikátoraiban.",
                2,
                4,
                90,
                null,
                "/images/missions/budai-kod.jpg",
                new GeoPoint(47.5020, 19.0344),
                120),
            new Mission(
                Guid.Parse("00000000-0000-0000-0000-000000000002"),
                "Éjféli frekvencia",
                "Fejtsetek meg egy évtizedek óta hallgató rádióadás üzenetét.",
                2,
                3,
                120,
                6990,
                "/images/missions/ejfeli-frekvencia.jpg",
                new GeoPoint(47.4979, 19.0402),
                100),
            new Mission(
                Guid.Parse("00000000-0000-0000-0000-000000000003"),
                "A Duna árnyéka",
                "Fényképek, hangok és rejtett jelek vezetnek át a rakparton.",
                3,
                5,
                105,
                8990,
                "/images/missions/duna-arnyeka.jpg",
                new GeoPoint(47.4847, 19.0526),
                140));

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
