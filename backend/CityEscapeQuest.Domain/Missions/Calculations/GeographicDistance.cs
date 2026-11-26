using CityEscapeQuest.Domain.Missions.ValueObjects;
namespace CityEscapeQuest.Domain.Missions.Calculations;

internal static class GeographicDistance
{
    public static double InMeters(GeoPoint origin, GeoPoint other)
    {
        const double earthRadiusMeters = 6_371_000;
        var latitudeDelta = DegreesToRadians(other.Latitude - origin.Latitude);
        var longitudeDelta = DegreesToRadians(other.Longitude - origin.Longitude);
        var firstLatitude = DegreesToRadians(origin.Latitude);
        var secondLatitude = DegreesToRadians(other.Latitude);

        var haversine = Math.Pow(Math.Sin(latitudeDelta / 2), 2)
            + Math.Cos(firstLatitude) * Math.Cos(secondLatitude)
            * Math.Pow(Math.Sin(longitudeDelta / 2), 2);

        return 2 * earthRadiusMeters * Math.Asin(Math.Sqrt(haversine));
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
}