namespace CityEscapeQuest.Application.GameSessions.Dtos;

public sealed record GamePlayState(
    Guid SessionId,
    string Status,
    string MissionTitle,
    string StoryIntro,
    string? TaskPrompt,
    string? CompletionText,
    DateTimeOffset? StartedAtUtc,
    DateTimeOffset? CompletedAtUtc);
