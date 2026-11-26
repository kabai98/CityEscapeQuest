using CityEscapeQuest.Domain.GameSessions.ValueObjects;
using CityEscapeQuest.Domain.Common;

namespace CityEscapeQuest.Domain.GameSessions.Rules;

internal static class GameStartPolicy
{
    internal static void EnsureCanStart(
        IReadOnlyCollection<Participant> participants,
        GameStartRequirements requirements,
        DateTimeOffset nowUtc)
    {
        if (participants.Count < requirements.MinimumPlayers)
        {
            throw new DomainException("The mission does not have enough participants.");
        }

        if (!requirements.IsLocationCheckEnabled)
        {
            return;
        }

        foreach (var participant in participants)
        {
            var location = participant.GetLocation()
                ?? throw new DomainException($"Location is missing for participant {participant.Id}.");
            if (location.MeasuredAtUtc < nowUtc - requirements.MaximumLocationAge)
            {
                throw new DomainException("Every participant must provide a fresh location.");
            }

            if (location.AccuracyMeters > requirements.MaximumAccuracyMeters)
            {
                throw new DomainException("Every participant must provide an accurate location.");
            }

            if (requirements.StartLocation.DistanceToInMeters(location.Position) > requirements.RadiusMeters)
            {
                throw new DomainException("Every participant must be inside the mission start area.");
            }
        }
    }
}
