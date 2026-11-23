using CityEscapeQuest.Application.Abstractions;
using CityEscapeQuest.Application.Missions;
using CityEscapeQuest.Domain.Missions;

namespace CityEscapeQuest.Application.UnitTests;

public sealed class MissionCatalogServiceTests
{
    [Fact]
    public async Task BrowseAsync_MapsMissionCardFields()
    {
        var mission = new Mission(
            Guid.NewGuid(),
            "Mission",
            "Description",
            2,
            4,
            60,
            null,
            "/preview.jpg",
            new GeoPoint(47.4979, 19.0402),
            100);
        var service = new MissionCatalogService(new FakeMissionRepository(mission));

        var result = await service.BrowseAsync(1, 20, CancellationToken.None);

        var card = Assert.Single(result);
        Assert.Equal("Mission", card.Title);
        Assert.True(card.IsFree);
        Assert.Equal(4, card.RecommendedPlayers);
    }

    private sealed class FakeMissionRepository(Mission mission) : IMissionRepository
    {
        public Task<IReadOnlyList<Mission>> BrowseAsync(int skip, int take, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<Mission>>([mission]);

        public Task<Mission?> GetAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult<Mission?>(mission.Id == id ? mission : null);
    }
}