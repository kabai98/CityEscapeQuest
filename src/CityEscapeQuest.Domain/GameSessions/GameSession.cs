using CityEscapeQuest.Domain.Common;
using CityEscapeQuest.Domain.Missions;

namespace CityEscapeQuest.Domain.GameSessions;

public sealed class GameSession
{
    private readonly List<Participant> _participants = [];

    private GameSession()
    {
    }

    private GameSession(
        Guid id,
        Guid missionId,
        string ownerUserId,
        string joinCodeHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset joinCodeExpiresAtUtc)
    {
        Id = id;
        MissionId = missionId;
        OwnerUserId = ownerUserId;
        JoinCodeHash = joinCodeHash;
        Status = GameSessionStatus.Lobby;
        CreatedAtUtc = createdAtUtc;
        JoinCodeExpiresAtUtc = joinCodeExpiresAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid MissionId { get; private set; }
    public string OwnerUserId { get; private set; } = string.Empty;
    public string JoinCodeHash { get; private set; } = string.Empty;
    public GameSessionStatus Status { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset JoinCodeExpiresAtUtc { get; private set; }
    public DateTimeOffset? StartedAtUtc { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }
    public int CurrentStage { get; private set; }
    public IReadOnlyCollection<Participant> Participants => _participants.AsReadOnly();

    public static GameSession Create(
        Guid missionId,
        string ownerUserId,
        string ownerDisplayName,
        string joinCodeHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset joinCodeExpiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(ownerUserId) || string.IsNullOrWhiteSpace(joinCodeHash))
        {
            throw new DomainException("Owner and join code are required.");
        }

        if (joinCodeExpiresAtUtc <= createdAtUtc)
        {
            throw new DomainException("Join code expiration must be after session creation.");
        }

        var session = new GameSession(
            Guid.NewGuid(),
            missionId,
            ownerUserId,
            joinCodeHash,
            createdAtUtc,
            joinCodeExpiresAtUtc);
        session.AddRegisteredParticipant(ownerUserId, ownerDisplayName);
        return session;
    }

    public Participant AddRegisteredParticipant(string userId, string displayName)
    {
        EnsureLobby();
        if (_participants.Any(participant => participant.UserId == userId))
        {
            throw new DomainException("The user has already joined this session.");
        }

        return AddParticipant(displayName, userId);
    }

    public Participant JoinRegisteredParticipant(string userId, string displayName, DateTimeOffset nowUtc)
    {
        EnsureJoinable(nowUtc);
        return AddRegisteredParticipant(userId, displayName);
    }

    public Participant AddGuest(string displayName, DateTimeOffset nowUtc)
    {
        EnsureJoinable(nowUtc);
        if (displayName.Length > 40)
        {
            throw new DomainException("Guest display name cannot exceed 40 characters.");
        }

        return AddParticipant(displayName, null);
    }

    public void ReportParticipantLocation(
        Guid participantId,
        double latitude,
        double longitude,
        double accuracyMeters,
        DateTimeOffset measuredAtUtc,
        DateTimeOffset receivedAtUtc)
    {
        EnsureLobby();
        var participant = _participants.SingleOrDefault(item => item.Id == participantId)
            ?? throw new DomainException("Participant was not found in this session.");
        participant.ReportLocation(latitude, longitude, accuracyMeters, measuredAtUtc, receivedAtUtc);
    }

    public void Start(
        GeoPoint missionStart,
        int radiusMeters,
        int minimumPlayers,
        DateTimeOffset nowUtc,
        TimeSpan maximumLocationAge,
        double maximumAccuracyMeters,
        bool isLocationCheckEnabled = true)
    {
        EnsureLobby();
        if (_participants.Count < minimumPlayers)
        {
            throw new DomainException("The mission does not have enough participants.");
        }

        if (isLocationCheckEnabled)
        {
            foreach (var participant in _participants)
            {
                var location = participant.GetLocation()
                    ?? throw new DomainException($"Location is missing for participant {participant.Id}.");
                if (location.MeasuredAtUtc < nowUtc - maximumLocationAge)
                    throw new DomainException("Every participant must provide a fresh location.");
                if (location.AccuracyMeters > maximumAccuracyMeters)
                    throw new DomainException("Every participant must provide an accurate location.");
                if (missionStart.DistanceToInMeters(location.Position) > radiusMeters)
                    throw new DomainException("Every participant must be inside the mission start area.");
            }
        }

        Status = GameSessionStatus.InProgress;
        StartedAtUtc = nowUtc;
        CurrentStage = 1;
    }

    public void Complete(DateTimeOffset nowUtc)
    {
        if (Status != GameSessionStatus.InProgress)
        {
            throw new DomainException("Only an active game session can be completed.");
        }

        Status = GameSessionStatus.Completed;
        CompletedAtUtc = nowUtc;
        CurrentStage = 2;
    }

    private Participant AddParticipant(string displayName, string? userId)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new DomainException("Display name is required.");
        }

        var normalizedName = displayName.Trim();
        if (_participants.Any(participant =>
                string.Equals(participant.DisplayName, normalizedName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DomainException("Display name is already used in this lobby.");
        }

        var participant = new Participant(Guid.NewGuid(), normalizedName, userId);
        _participants.Add(participant);
        return participant;
    }

    private void EnsureLobby()
    {
        if (Status != GameSessionStatus.Lobby)
        {
            throw new DomainException("The game session is not in the lobby state.");
        }
    }

    private void EnsureJoinable(DateTimeOffset nowUtc)
    {
        EnsureLobby();
        if (nowUtc >= JoinCodeExpiresAtUtc)
        {
            throw new DomainException("Join code has expired.");
        }
    }
}
