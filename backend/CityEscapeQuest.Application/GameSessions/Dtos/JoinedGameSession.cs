namespace CityEscapeQuest.Application.GameSessions.Dtos;

public sealed record JoinedGameSession(
    Guid SessionId,
    Guid ParticipantId,
    string? AccessToken,
    DateTimeOffset? AccessTokenExpiresAtUtc);
