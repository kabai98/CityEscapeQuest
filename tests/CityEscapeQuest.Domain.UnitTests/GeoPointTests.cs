using CityEscapeQuest.Domain.Missions.ValueObjects;

namespace CityEscapeQuest.Domain.UnitTests;

public sealed class GeoPointTests
{
    [Fact]
    public void DistanceToInMeters_NearbyBudapestLocations_ReturnsExpectedDistance()
    {
        var first = new GeoPoint(47.4979, 19.0402);
        var second = new GeoPoint(47.4988, 19.0415);

        var distance = first.DistanceToInMeters(second);

        Assert.InRange(distance, 130, 150);
    }
}