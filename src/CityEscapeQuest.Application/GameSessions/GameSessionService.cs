using System.Security.Cryptography;
using System.Text;
using CityEscapeQuest.Application.Abstractions;
using CityEscapeQuest.Domain.GameSessions;
using CityEscapeQuest.Domain.Missions;

namespace CityEscapeQuest.Application.GameSessions;

public sealed class GameSessionService(
    IGameSessionRepository sessions,
    IMissionRepository missions,
    IGuestAccessTokenService guestAccessTokens,
    TimeProvider timeProvider)
{
    public async Task<CreatedGameSession> CreateAsync(
        Guid missionId,
        string ownerUserId,
        string ownerDisplayName,
        CancellationToken cancellationToken)
    {
        _ = await missions.GetAsync(missionId, cancellationToken)
            ?? throw new KeyNotFoundException("Mission was not found.");

        var joinCode = CreateJoinCode();
        var now = timeProvider.GetUtcNow();
        var session = GameSession.Create(
            missionId,
            ownerUserId,
            ownerDisplayName,
            Hash(joinCode),
            now,
            now.AddHours(2));
        await sessions.AddAsync(session, cancellationToken);
        await sessions.SaveChangesAsync(cancellationToken);
        return new CreatedGameSession(session.Id, joinCode, session.JoinCodeExpiresAtUtc);
    }

    public async Task<JoinedGameSession> JoinAsync(
        string joinCode,
        string displayName,
        string? registeredUserId,
        CancellationToken cancellationToken)
    {
        var session = await sessions.GetByJoinCodeHashAsync(Hash(joinCode), cancellationToken)
            ?? throw new KeyNotFoundException("Game session was not found.");
        var now = timeProvider.GetUtcNow();

        if (!string.IsNullOrWhiteSpace(registeredUserId))
        {
            var participant = session.JoinRegisteredParticipant(registeredUserId, displayName, now);
            await sessions.SaveChangesAsync(cancellationToken);
            return new JoinedGameSession(session.Id, participant.Id, null, null);
        }

        var guest = session.AddGuest(displayName, now);
        var token = guestAccessTokens.Issue(session.Id, guest.Id, guest.DisplayName);
        await sessions.SaveChangesAsync(cancellationToken);
        return new JoinedGameSession(session.Id, guest.Id, token.AccessToken, token.ExpiresAtUtc);
    }

    public async Task<GameSessionLobby> GetLobbyAsync(
        Guid sessionId,
        string subjectId,
        Guid? guestParticipantId,
        CancellationToken cancellationToken)
    {
        var session = await sessions.GetAsync(sessionId, cancellationToken)
            ?? throw new KeyNotFoundException("Game session was not found.");

        var canView = string.Equals(session.OwnerUserId, subjectId, StringComparison.Ordinal)
            || session.Participants.Any(participant =>
                string.Equals(participant.UserId, subjectId, StringComparison.Ordinal)
                || participant.Id == guestParticipantId);
        if (!canView)
        {
            throw new UnauthorizedAccessException("The caller is not a participant of this game session.");
        }

        return new GameSessionLobby(
            session.Id,
            session.MissionId,
            session.Status.ToString(),
            session.JoinCodeExpiresAtUtc,
            session.Participants.Select(participant => new LobbyParticipant(
                participant.Id,
                participant.DisplayName,
                participant.IsGuest,
                string.Equals(participant.UserId, session.OwnerUserId, StringComparison.Ordinal))).ToArray());
    }

    public async Task StartAsync(
        Guid sessionId,
        string requestingUserId,
        IReadOnlyCollection<LocationInput> locations,
        CancellationToken cancellationToken)
    {
        var session = await sessions.GetAsync(sessionId, cancellationToken)
            ?? throw new KeyNotFoundException("Game session was not found.");

        if (!string.Equals(session.OwnerUserId, requestingUserId, StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException("Only the team owner can start the game.");
        }

        var mission = await missions.GetAsync(session.MissionId, cancellationToken)
            ?? throw new KeyNotFoundException("Mission was not found.");

        var now = timeProvider.GetUtcNow();
        var domainLocations = locations.Select(location => new ParticipantLocation(
            location.ParticipantId,
            new GeoPoint(location.Latitude, location.Longitude),
            location.AccuracyMeters,
            location.MeasuredAtUtc)).ToArray();

        session.Start(
            mission.StartLocation,
            mission.StartRadiusMeters,
            mission.MinimumPlayers,
            domainLocations,
            now,
            TimeSpan.FromMinutes(2),
            maximumAccuracyMeters: 50);

        await sessions.SaveChangesAsync(cancellationToken);
    }

    private static string CreateJoinCode() => RandomNumberGenerator.GetInt32(100_000, 1_000_000).ToString();

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}

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
    IReadOnlyCollection<LobbyParticipant> Participants);
public sealed record LobbyParticipant(Guid Id, string DisplayName, bool IsGuest, bool IsOwner);

public sealed record LocationInput(
    Guid ParticipantId,
    double Latitude,
    double Longitude,
    double AccuracyMeters,
    DateTimeOffset MeasuredAtUtc);
