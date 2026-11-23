using CityEscapeQuest.Domain.Missions;

namespace CityEscapeQuest.Domain.GameSessions;

public sealed record ParticipantLocation(
    Guid ParticipantId,
    GeoPoint Position,
    double AccuracyMeters,
    DateTimeOffset MeasuredAtUtc);