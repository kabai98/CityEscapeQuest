using CityEscapeQuest.Domain.GameSessions;

namespace CityEscapeQuest.Application.Abstractions;

public interface IGameSessionRepository
{
    Task AddAsync(GameSession session, CancellationToken cancellationToken);
    Task<GameSession?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<GameSession?> GetByJoinCodeHashAsync(string joinCodeHash, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
