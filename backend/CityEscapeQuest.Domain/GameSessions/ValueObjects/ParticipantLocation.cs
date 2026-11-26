using CityEscapeQuest.Domain.Missions.ValueObjects;
using CityEscapeQuest.Domain.Missions;

namespace CityEscapeQuest.Domain.GameSessions.ValueObjects;

public sealed record ParticipantLocation(
    Guid ParticipantId,
    GeoPoint Position,
    double AccuracyMeters,
    DateTimeOffset MeasuredAtUtc);