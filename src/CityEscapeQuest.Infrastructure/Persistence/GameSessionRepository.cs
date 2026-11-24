using CityEscapeQuest.Application.Abstractions;
using CityEscapeQuest.Domain.GameSessions;
using Microsoft.EntityFrameworkCore;

namespace CityEscapeQuest.Infrastructure.Persistence;

internal sealed class GameSessionRepository(AppDbContext dbContext) : IGameSessionRepository
{
    public Task AddAsync(GameSession session, CancellationToken cancellationToken) =>
        dbContext.GameSessions.AddAsync(session, cancellationToken).AsTask();

    public Task<GameSession?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.GameSessions
            .Include(session => session.Participants)
            .SingleOrDefaultAsync(session => session.Id == id, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}