using CityEscapeQuest.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CityEscapeQuest.Application.Missions;

public sealed class MissionCatalogService(IApplicationDbContext dbContext)
{
    private const int MaximumPageSize = 50;

    public async Task<IReadOnlyList<MissionSummary>> BrowseAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var normalizedPage = Math.Max(page, 1);
        var normalizedPageSize = Math.Clamp(pageSize, 1, MaximumPageSize);
        return await dbContext.Missions
            .AsNoTracking()
            .Where(mission => mission.IsPublished)
            .OrderBy(mission => mission.Title)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(mission => new MissionSummary(
                mission.Id,
                mission.Title,
                mission.ShortDescription,
                mission.RecommendedPlayers,
                mission.EstimatedDurationMinutes,
                mission.Price,
                mission.Price == null || mission.Price == 0,
                mission.PreviewImageUrl))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<MissionDetails?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Missions
            .AsNoTracking()
            .Where(mission => mission.Id == id)
            .Select(mission => new MissionDetails(
                mission.Id,
                mission.Title,
                mission.ShortDescription,
                mission.MinimumPlayers,
                mission.RecommendedPlayers,
                mission.EstimatedDurationMinutes,
                mission.Price,
                mission.Price == null || mission.Price == 0,
                mission.PreviewImageUrl,
                mission.StartLocation.Latitude,
                mission.StartLocation.Longitude,
                mission.StartLocation.RadiusMeters,
                mission.IsLocationCheckEnabled))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
