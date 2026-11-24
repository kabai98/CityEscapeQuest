using CityEscapeQuest.Domain.Common;
using CityEscapeQuest.Domain.GameSessions;
using CityEscapeQuest.Domain.Missions;

namespace CityEscapeQuest.Domain.UnitTests;

public sealed class GameSessionTests
{
    [Fact]
    public void Start_WhenEveryParticipantIsInsideStartArea_StartsGame()
    {
        var session = GameSession.Create(Guid.NewGuid(), "owner", "Owner", "hash");
        var guest = session.AddGuest("Guest");
        var now = DateTimeOffset.UtcNow;
        var locations = session.Participants.Select(participant => new ParticipantLocation(
            participant.Id,
            new GeoPoint(47.4979, 19.0402),
            10,
            now)).ToArray();

        session.Start(new GeoPoint(47.4979, 19.0402), 100, 2, locations, now, TimeSpan.FromMinutes(2), 50);

        Assert.Equal(GameSessionStatus.InProgress, session.Status);
        Assert.NotNull(session.StartedAtUtc);
        Assert.Contains(session.Participants, participant => participant.Id == guest.Id);
    }

    [Fact]
    public void Start_WhenParticipantIsOutsideStartArea_ThrowsDomainException()
    {
        var session = GameSession.Create(Guid.NewGuid(), "owner", "Owner", "hash");
        session.AddGuest("Guest");
        var now = DateTimeOffset.UtcNow;
        var locations = session.Participants.Select(participant => new ParticipantLocation(
            participant.Id,
            new GeoPoint(47.5100, 19.0600),
            10,
            now)).ToArray();

        var exception = Assert.Throws<DomainException>(() => session.Start(
            new GeoPoint(47.4979, 19.0402),
            100,
            2,
            locations,
            now,
            TimeSpan.FromMinutes(2),
            50));

        Assert.Contains("inside", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}