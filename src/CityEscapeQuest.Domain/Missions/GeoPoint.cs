using CityEscapeQuest.Domain.Common;

namespace CityEscapeQuest.Domain.Missions;

public sealed record GeoPoint
{
    private GeoPoint()
    {
    }

    public GeoPoint(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
        {
            throw new DomainException("Latitude must be between -90 and 90 degrees.");
        }

        if (longitude is < -180 or > 180)
        {
            throw new DomainException("Longitude must be between -180 and 180 degrees.");
        }

        Latitude = latitude;
        Longitude = longitude;
    }

    public double Latitude { get; private set; }

    public double Longitude { get; private set; }

    public double DistanceToInMeters(GeoPoint other)
    {
        const double earthRadiusMeters = 6_371_000;
        var latitudeDelta = DegreesToRadians(other.Latitude - Latitude);
        var longitudeDelta = DegreesToRadians(other.Longitude - Longitude);
        var firstLatitude = DegreesToRadians(Latitude);
        var secondLatitude = DegreesToRadians(other.Latitude);

        var haversine = Math.Pow(Math.Sin(latitudeDelta / 2), 2)
            + Math.Cos(firstLatitude) * Math.Cos(secondLatitude)
            * Math.Pow(Math.Sin(longitudeDelta / 2), 2);

        return 2 * earthRadiusMeters * Math.Asin(Math.Sqrt(haversine));
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
}