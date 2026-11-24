using CityEscapeQuest.Application.Abstractions;
using CityEscapeQuest.Domain.Missions;
using Microsoft.EntityFrameworkCore;

namespace CityEscapeQuest.Infrastructure.Persistence;

internal sealed class MissionRepository(AppDbContext dbContext) : IMissionRepository
{
    public async Task<IReadOnlyList<Mission>> BrowseAsync(int skip, int take, CancellationToken cancellationToken) =>
        await dbContext.Missions
            .AsNoTracking()
            .Where(mission => mission.IsPublished)
            .OrderBy(mission => mission.Title)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

    public Task<Mission?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Missions.AsNoTracking().SingleOrDefaultAsync(mission => mission.Id == id, cancellationToken);
}