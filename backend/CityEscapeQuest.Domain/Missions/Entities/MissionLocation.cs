using CityEscapeQuest.Domain.Missions.ValueObjects;
namespace CityEscapeQuest.Domain.Missions;

public sealed class MissionLocation
{
    private MissionLocation()
    {
    }

    public MissionLocation(Guid id, GeoPoint coordinates, int radiusMeters)
    {
        if (radiusMeters < 1)
        {
            throw new Common.DomainException("Start radius must be positive.");
        }

        Id = id;
        Latitude = coordinates.Latitude;
        Longitude = coordinates.Longitude;
        RadiusMeters = radiusMeters;
    }

    public Guid Id { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public int RadiusMeters { get; private set; }
    public GeoPoint Coordinates => new(Latitude, Longitude);
}
