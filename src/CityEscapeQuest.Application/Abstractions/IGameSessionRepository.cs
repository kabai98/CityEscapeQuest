using CityEscapeQuest.Domain.GameSessions;

namespace CityEscapeQuest.Application.Abstractions;

public interface IGameSessionRepository
{
    Task AddAsync(GameSession session, CancellationToken cancellationToken);
    Task<GameSession?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}