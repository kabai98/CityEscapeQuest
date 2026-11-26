using CityEscapeQuest.Domain.Missions.ValueObjects;
using CityEscapeQuest.Domain.Missions;

namespace CityEscapeQuest.Domain.GameSessions.ValueObjects;

public sealed record GameStartRequirements(
    GeoPoint StartLocation,
    int RadiusMeters,
    int MinimumPlayers,
    TimeSpan MaximumLocationAge,
    double MaximumAccuracyMeters,
    bool IsLocationCheckEnabled = true);
