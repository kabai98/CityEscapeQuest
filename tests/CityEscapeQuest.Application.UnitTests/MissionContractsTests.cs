using CityEscapeQuest.Application.Missions;
using CityEscapeQuest.Domain.Missions;

namespace CityEscapeQuest.Application.UnitTests;

public sealed class MissionContractsTests
{
    [Fact]
    public void FromMission_MapsMissionCardFields()
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
        var card = MissionSummary.FromMission(mission);
        Assert.Equal("Mission", card.Title);
        Assert.True(card.IsFree);
        Assert.Equal(4, card.RecommendedPlayers);
    }

}
