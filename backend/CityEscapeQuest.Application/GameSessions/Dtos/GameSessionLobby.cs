namespace CityEscapeQuest.Application.GameSessions.Dtos;

public sealed record GameSessionLobby(
    Guid SessionId,
    Guid MissionId,
    string Status,
    DateTimeOffset JoinCodeExpiresAtUtc,
    bool IsLocationCheckEnabled,
    IReadOnlyCollection<LobbyParticipant> Participants);
