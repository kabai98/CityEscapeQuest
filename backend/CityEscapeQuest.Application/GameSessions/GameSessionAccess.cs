using CityEscapeQuest.Domain.GameSessions;

namespace CityEscapeQuest.Application.GameSessions;

internal static class GameSessionAccess
{
    public static Participant GetCurrentParticipant(GameSession session, string subjectId, Guid? guestParticipantId) =>
        session.Participants.SingleOrDefault(participant =>
            string.Equals(participant.UserId, subjectId, StringComparison.Ordinal)
            || participant.Id == guestParticipantId)
        ?? throw new UnauthorizedAccessException("The caller is not a participant of this game session.");

    public static bool IsCurrentParticipant(Participant participant, string subjectId, Guid? guestParticipantId) =>
        string.Equals(participant.UserId, subjectId, StringComparison.Ordinal)
        || participant.Id == guestParticipantId;

}
