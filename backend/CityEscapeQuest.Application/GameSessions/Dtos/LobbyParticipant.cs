namespace CityEscapeQuest.Application.GameSessions.Dtos;

public sealed record LobbyParticipant(
    Guid Id,
    string DisplayName,
    bool IsGuest,
    bool IsOwner,
    bool IsCurrent,
    bool IsLocationReady);
