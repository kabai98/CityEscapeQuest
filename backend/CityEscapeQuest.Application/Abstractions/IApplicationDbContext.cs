using CityEscapeQuest.Domain.Achievements;
using CityEscapeQuest.Domain.GameSessions;
using CityEscapeQuest.Domain.Missions;
using Microsoft.EntityFrameworkCore;

namespace CityEscapeQuest.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Mission> Missions { get; }
    DbSet<GameSession> GameSessions { get; }
    DbSet<Achievement> Achievements { get; }
    DbSet<MissionLocation> MissionLocations { get; }
    DbSet<Participant> GameSessionParticipants { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
