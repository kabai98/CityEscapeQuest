namespace CityEscapeQuest.Application.GameSessions.Dtos;

public sealed record LocationInput(
    double Latitude,
    double Longitude,
    double AccuracyMeters,
    DateTimeOffset MeasuredAtUtc);
