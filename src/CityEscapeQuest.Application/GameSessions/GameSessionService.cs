using System.Security.Cryptography;
using System.Text;
using CityEscapeQuest.Application.Abstractions;
using CityEscapeQuest.Domain.GameSessions;
using CityEscapeQuest.Domain.Missions;

namespace CityEscapeQuest.Application.GameSessions;

public sealed class GameSessionService(
    IGameSessionRepository sessions,
    IMissionRepository missions,
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
        var session = GameSession.Create(missionId, ownerUserId, ownerDisplayName, Hash(joinCode));
        await sessions.AddAsync(session, cancellationToken);
        await sessions.SaveChangesAsync(cancellationToken);
        return new CreatedGameSession(session.Id, joinCode);
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

public sealed record CreatedGameSession(Guid SessionId, string JoinCode);

public sealed record LocationInput(
    Guid ParticipantId,
    double Latitude,
    double Longitude,
    double AccuracyMeters,
    DateTimeOffset MeasuredAtUtc);