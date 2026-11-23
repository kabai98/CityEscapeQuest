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
    public bool IsGuest => UserId is null;
}