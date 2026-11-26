using CityEscapeQuest.Domain.Missions.ValueObjects;

namespace CityEscapeQuest.Domain.GameSessions.ValueObjects;

public sealed record ParticipantLocation(
    Guid ParticipantId,
    GeoPoint Position,
    double AccuracyMeters,
    DateTimeOffset MeasuredAtUtc);