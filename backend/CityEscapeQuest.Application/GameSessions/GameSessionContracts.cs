namespace CityEscapeQuest.Application.GameSessions;

public sealed record CreatedGameSession(Guid SessionId, string JoinCode, DateTimeOffset JoinCodeExpiresAtUtc);

public sealed record JoinedGameSession(
    Guid SessionId,
    Guid ParticipantId,
    string? AccessToken,
    DateTimeOffset? AccessTokenExpiresAtUtc);

public sealed record GameSessionLobby(
    Guid SessionId,
    Guid MissionId,
    string Status,
    DateTimeOffset JoinCodeExpiresAtUtc,
    bool IsLocationCheckEnabled,
    IReadOnlyCollection<LobbyParticipant> Participants);

public sealed record LobbyParticipant(
    Guid Id,
    string DisplayName,
    bool IsGuest,
    bool IsOwner,
    bool IsCurrent,
    bool IsLocationReady);

public sealed record GamePlayState(
    Guid SessionId,
    string Status,
    string MissionTitle,
    string StoryIntro,
    string? TaskPrompt,
    string? CompletionText,
    DateTimeOffset? StartedAtUtc,
    DateTimeOffset? CompletedAtUtc);

public sealed record LocationInput(
    double Latitude,
    double Longitude,
    double AccuracyMeters,
    DateTimeOffset MeasuredAtUtc);
