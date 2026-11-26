using CityEscapeQuest.Domain.Common;

namespace CityEscapeQuest.Domain.GameSessions.Rules;

internal static class ParticipantRegistrationPolicy
{
    public static Participant CreateParticipant(
        IReadOnlyCollection<Participant> participants, string displayName, string? userId)
    {
        if (userId is not null && participants.Any(participant => participant.UserId == userId))
        {
            throw new DomainException("The user has already joined this session.");
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new DomainException("Display name is required.");
        }

        if (userId is null && displayName.Length > 40)
        {
            throw new DomainException("Guest display name cannot exceed 40 characters.");
        }

        var normalizedName = displayName.Trim();
        if (participants.Any(participant =>
                string.Equals(participant.DisplayName, normalizedName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DomainException("Display name is already used in this lobby.");
        }

        return new Participant(Guid.NewGuid(), normalizedName, userId);
    }
}
