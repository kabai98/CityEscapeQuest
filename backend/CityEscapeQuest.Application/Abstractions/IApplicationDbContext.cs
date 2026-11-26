using CityEscapeQuest.Domain.GameSessions;
using CityEscapeQuest.Domain.Missions;
using Microsoft.EntityFrameworkCore;

namespace CityEscapeQuest.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Mission> Missions { get; }
    DbSet<GameSession> GameSessions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
