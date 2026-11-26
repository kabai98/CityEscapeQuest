using CityEscapeQuest.Domain.Missions.Calculations;
using CityEscapeQuest.Domain.Common;

namespace CityEscapeQuest.Domain.Missions.ValueObjects;

public sealed record GeoPoint
{
    private const double MinimumLatitude = -90;
    private const double MaximumLatitude = 90;
    private const double MinimumLongitude = -180;
    private const double MaximumLongitude = 180;

    private GeoPoint()
    {
    }

    public GeoPoint(double latitude, double longitude)
    {
        if (latitude is < MinimumLatitude or > MaximumLatitude)
        {
            throw new DomainException("Latitude must be between -90 and 90 degrees.");
        }

        if (longitude is < MinimumLongitude or > MaximumLongitude)
        {
            throw new DomainException("Longitude must be between -180 and 180 degrees.");
        }

        Latitude = latitude;
        Longitude = longitude;
    }

    public double Latitude { get; private set; }

    public double Longitude { get; private set; }

    public double DistanceToInMeters(GeoPoint other) => GeographicDistance.InMeters(this, other);
}
