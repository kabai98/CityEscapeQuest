using CityEscapeQuest.Application.Abstractions;

namespace CityEscapeQuest.Application.Missions;

public sealed class MissionCatalogService(IMissionRepository missions)
{
    private const int MaximumPageSize = 50;

    public async Task<IReadOnlyList<MissionSummary>> BrowseAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var normalizedPage = Math.Max(page, 1);
        var normalizedPageSize = Math.Clamp(pageSize, 1, MaximumPageSize);
        var result = await missions.BrowseAsync(
            (normalizedPage - 1) * normalizedPageSize,
            normalizedPageSize,
            cancellationToken);

        return result.Select(MissionSummary.FromMission).ToArray();
    }

    public async Task<MissionDetails?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var mission = await missions.GetAsync(id, cancellationToken);
        return mission is null ? null : MissionDetails.FromMission(mission);
    }
}