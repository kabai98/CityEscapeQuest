using CityEscapeQuest.Application.GameSessions.Dtos;
using CityEscapeQuest.Domain.GameSessions.ValueObjects;
using CityEscapeQuest.Application.Abstractions;
using CityEscapeQuest.Domain.GameSessions;
using CityEscapeQuest.Domain.Missions;
using Microsoft.EntityFrameworkCore;

namespace CityEscapeQuest.Application.GameSessions;

public sealed class GameSessionPlayService(
    IApplicationDbContext dbContext,
    TimeProvider timeProvider)
{
    public async Task<GamePlayState> StartAsync(
        Guid sessionId,
        string requestingUserId,
        CancellationToken cancellationToken)
    {
        var session = await GetSessionAsync(sessionId, cancellationToken)
            ?? throw new KeyNotFoundException("Game session was not found.");

        if (!string.Equals(session.OwnerUserId, requestingUserId, StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException("Only the team owner can start the game.");
        }

        var mission = await GetMissionAsync(session.MissionId, cancellationToken)
            ?? throw new KeyNotFoundException("Mission was not found.");

        var now = timeProvider.GetUtcNow();
        var requirements = new GameStartRequirements(
            mission.StartLocation.Coordinates,
            mission.StartRadiusMeters,
            mission.MinimumPlayers,
            TimeSpan.FromMinutes(2),
            50,
            mission.IsLocationCheckEnabled);
        session.Start(requirements, now);

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToGamePlayState(session, mission);
    }

    public async Task<GamePlayState> GetGameStateAsync(
        Guid sessionId,
        string subjectId,
        Guid? guestParticipantId,
        CancellationToken cancellationToken)
    {
        var session = await GetReadOnlySessionAsync(sessionId, cancellationToken)
            ?? throw new KeyNotFoundException("Game session was not found.");
        _ = GameSessionAccess.GetCurrentParticipant(session, subjectId, guestParticipantId);
        var mission = await GetMissionAsync(session.MissionId, cancellationToken)
            ?? throw new KeyNotFoundException("Mission was not found.");
        return ToGamePlayState(session, mission);
    }

    public async Task<GamePlayState> SubmitAnswerAsync(
        Guid sessionId,
        string subjectId,
        Guid? guestParticipantId,
        string answer,
        CancellationToken cancellationToken)
    {
        var session = await GetSessionAsync(sessionId, cancellationToken)
            ?? throw new KeyNotFoundException("Game session was not found.");
        _ = GameSessionAccess.GetCurrentParticipant(session, subjectId, guestParticipantId);
        var mission = await GetMissionAsync(session.MissionId, cancellationToken)
            ?? throw new KeyNotFoundException("Mission was not found.");
        if (!mission.IsCorrectAnswer(answer))
        {
            throw new Domain.Common.DomainException("The submitted answer is not correct.");
        }

        session.Complete(timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToGamePlayState(session, mission);
    }

    private Task<GameSession?> GetSessionAsync(Guid sessionId, CancellationToken cancellationToken) =>
        dbContext.GameSessions.Include(session => session.Participants)
            .SingleOrDefaultAsync(session => session.Id == sessionId, cancellationToken);

    private Task<GameSession?> GetReadOnlySessionAsync(Guid sessionId, CancellationToken cancellationToken) =>
        dbContext.GameSessions.AsNoTracking().Include(session => session.Participants)
            .SingleOrDefaultAsync(session => session.Id == sessionId, cancellationToken);

    private Task<Mission?> GetMissionAsync(Guid missionId, CancellationToken cancellationToken) =>
        dbContext.Missions.AsNoTracking().Include(mission => mission.StartLocation)
            .SingleOrDefaultAsync(mission => mission.Id == missionId, cancellationToken);

    private static GamePlayState ToGamePlayState(GameSession session, Mission mission) => new(
        session.Id,
        session.Status.ToString(),
        mission.Title,
        mission.StoryIntro,
        session.Status == GameSessionStatus.InProgress ? mission.TaskPrompt : null,
        session.Status == GameSessionStatus.Completed ? mission.CompletionText : null,
        session.StartedAtUtc,
        session.CompletedAtUtc);

}
