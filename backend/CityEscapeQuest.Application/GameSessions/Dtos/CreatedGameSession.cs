namespace CityEscapeQuest.Application.GameSessions.Dtos;

public sealed record CreatedGameSession(Guid SessionId, string JoinCode, DateTimeOffset JoinCodeExpiresAtUtc);
