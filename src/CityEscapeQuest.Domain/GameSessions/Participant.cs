namespace CityEscapeQuest.Domain.GameSessions;

public sealed class Participant
{
    private Participant()
    {
    }

    internal Participant(Guid id, string displayName, string? userId)
    {
        Id = id;
        DisplayName = displayName.Trim();
        UserId = userId;
    }

    public Guid Id { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public string? UserId { get; private set; }
    public double? LastLatitude { get; private set; }
    public double? LastLongitude { get; private set; }
    public double? LastAccuracyMeters { get; private set; }
    public DateTimeOffset? LocationMeasuredAtUtc { get; private set; }
    public DateTimeOffset? LocationReceivedAtUtc { get; private set; }
    public bool IsGuest => UserId is null;

    public void ReportLocation(
        double latitude,
        double longitude,
        double accuracyMeters,
        DateTimeOffset measuredAtUtc,
        DateTimeOffset receivedAtUtc)
    {
        _ = new Missions.GeoPoint(latitude, longitude);
        if (accuracyMeters <= 0 || measuredAtUtc > receivedAtUtc.AddMinutes(1))
        {
            throw new Common.DomainException("Location evidence is invalid.");
        }

        LastLatitude = latitude;
        LastLongitude = longitude;
        LastAccuracyMeters = accuracyMeters;
        LocationMeasuredAtUtc = measuredAtUtc;
        LocationReceivedAtUtc = receivedAtUtc;
    }

    public ParticipantLocation? GetLocation()
    {
        if (LastLatitude is null || LastLongitude is null || LastAccuracyMeters is null || LocationMeasuredAtUtc is null)
        {
            return null;
        }

        return new ParticipantLocation(
            Id,
            new Missions.GeoPoint(LastLatitude.Value, LastLongitude.Value),
            LastAccuracyMeters.Value,
            LocationMeasuredAtUtc.Value);
    }

    public bool IsLocationReady(DateTimeOffset nowUtc, TimeSpan maximumAge, double maximumAccuracyMeters) =>
        GetLocation() is { } location
        && location.MeasuredAtUtc >= nowUtc - maximumAge
        && location.AccuracyMeters <= maximumAccuracyMeters;
}
