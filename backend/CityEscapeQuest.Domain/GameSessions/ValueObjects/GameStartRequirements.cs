using CityEscapeQuest.Domain.Missions.ValueObjects;

namespace CityEscapeQuest.Domain.GameSessions.ValueObjects;

public sealed record GameStartRequirements(
    GeoPoint StartLocation,
    int RadiusMeters,
    int MinimumPlayers,
    TimeSpan MaximumLocationAge,
    double MaximumAccuracyMeters,
    bool IsLocationCheckEnabled = true);
