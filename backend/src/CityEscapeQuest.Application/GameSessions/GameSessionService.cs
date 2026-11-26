using System.Security.Cryptography;
using System.Text;
using CityEscapeQuest.Application.Abstractions;
using CityEscapeQuest.Domain.GameSessions;
using CityEscapeQuest.Domain.Missions;
using Microsoft.EntityFrameworkCore;

namespace CityEscapeQuest.Application.GameSessions;

public sealed class GameSessionService(
    IApplicationDbContext dbContext,
    IGuestAccessTokenService guestAccessTokens,
    TimeProvider timeProvider)
{
    public async Task<CreatedGameSession> CreateAsync(
        Guid missionId,
        string ownerUserId,
        string ownerDisplayName,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Missions.AnyAsync(mission => mission.Id == missionId, cancellationToken))
        {
            throw new KeyNotFoundException("Mission was not found.");
        }

        var joinCode = CreateJoinCode();
        var now = timeProvider.GetUtcNow();
        var session = GameSession.Create(
            missionId,
            ownerUserId,
            ownerDisplayName,
            Hash(joinCode),
            now,
            now.AddHours(2));
        await dbContext.GameSessions.AddAsync(session, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreatedGameSession(session.Id, joinCode, session.JoinCodeExpiresAtUtc);
    }

    public async Task<JoinedGameSession> JoinAsync(
        string joinCode,
        string displayName,
        string? registeredUserId,
        CancellationToken cancellationToken)
    {
        var joinCodeHash = Hash(joinCode);
        var session = await dbContext.GameSessions.Include(gameSession => gameSession.Participants)
            .SingleOrDefaultAsync(gameSession => gameSession.JoinCodeHash == joinCodeHash, cancellationToken)
            ?? throw new KeyNotFoundException("Game session was not found.");
        var now = timeProvider.GetUtcNow();

        if (!string.IsNullOrWhiteSpace(registeredUserId))
        {
            var participant = session.JoinRegisteredParticipant(registeredUserId, displayName, now);
            await dbContext.SaveChangesAsync(cancellationToken);
            return new JoinedGameSession(session.Id, participant.Id, null, null);
        }

        var guest = session.AddGuest(displayName, now);
        var token = guestAccessTokens.Issue(session.Id, guest.Id, guest.DisplayName);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new JoinedGameSession(session.Id, guest.Id, token.AccessToken, token.ExpiresAtUtc);
    }

    public async Task<GameSessionLobby> GetLobbyAsync(
        Guid sessionId,
        string subjectId,
        Guid? guestParticipantId,
        CancellationToken cancellationToken)
    {
        var session = await GetReadOnlySessionAsync(sessionId, cancellationToken)
            ?? throw new KeyNotFoundException("Game session was not found.");

        var canView = string.Equals(session.OwnerUserId, subjectId, StringComparison.Ordinal)
            || session.Participants.Any(participant =>
                string.Equals(participant.UserId, subjectId, StringComparison.Ordinal)
                || participant.Id == guestParticipantId);
        if (!canView)
        {
            throw new UnauthorizedAccessException("The caller is not a participant of this game session.");
        }

        var mission = await GetMissionAsync(session.MissionId, cancellationToken)
            ?? throw new KeyNotFoundException("Mission was not found.");
        return new GameSessionLobby(
            session.Id,
            session.MissionId,
            session.Status.ToString(),
            session.JoinCodeExpiresAtUtc,
            mission.IsLocationCheckEnabled,
            session.Participants.Select(participant => new LobbyParticipant(
                participant.Id,
                participant.DisplayName,
                participant.IsGuest,
                string.Equals(participant.UserId, session.OwnerUserId, StringComparison.Ordinal),
                IsCurrentParticipant(participant, subjectId, guestParticipantId),
                participant.IsLocationReady(timeProvider.GetUtcNow(), TimeSpan.FromMinutes(2), 50))).ToArray());
    }

    public async Task ReportLocationAsync(
        Guid sessionId,
        string subjectId,
        Guid? guestParticipantId,
        LocationInput location,
        CancellationToken cancellationToken)
    {
        var session = await GetSessionAsync(sessionId, cancellationToken)
            ?? throw new KeyNotFoundException("Game session was not found.");
        var participant = GetCurrentParticipant(session, subjectId, guestParticipantId);
        var now = timeProvider.GetUtcNow();
        session.ReportParticipantLocation(
            participant.Id,
            location.Latitude,
            location.Longitude,
            location.AccuracyMeters,
            location.MeasuredAtUtc,
            now);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

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
        session.Start(
            mission.StartLocation.Coordinates,
            mission.StartRadiusMeters,
            mission.MinimumPlayers,
            now,
            TimeSpan.FromMinutes(2),
            maximumAccuracyMeters: 50,
            isLocationCheckEnabled: mission.IsLocationCheckEnabled);

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
        _ = GetCurrentParticipant(session, subjectId, guestParticipantId);
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
        _ = GetCurrentParticipant(session, subjectId, guestParticipantId);
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

    private static Participant GetCurrentParticipant(GameSession session, string subjectId, Guid? guestParticipantId) =>
        session.Participants.SingleOrDefault(participant =>
            string.Equals(participant.UserId, subjectId, StringComparison.Ordinal)
            || participant.Id == guestParticipantId)
        ?? throw new UnauthorizedAccessException("The caller is not a participant of this game session.");

    private static bool IsCurrentParticipant(Participant participant, string subjectId, Guid? guestParticipantId) =>
        string.Equals(participant.UserId, subjectId, StringComparison.Ordinal)
        || participant.Id == guestParticipantId;

    private static GamePlayState ToGamePlayState(GameSession session, Mission mission) => new(
        session.Id,
        session.Status.ToString(),
        mission.Title,
        mission.StoryIntro,
        session.Status == GameSessionStatus.InProgress ? mission.TaskPrompt : null,
        session.Status == GameSessionStatus.Completed ? mission.CompletionText : null,
        session.StartedAtUtc,
        session.CompletedAtUtc);

    private static string CreateJoinCode() => RandomNumberGenerator.GetInt32(100_000, 1_000_000).ToString();

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
