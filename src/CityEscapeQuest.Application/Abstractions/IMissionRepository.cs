using CityEscapeQuest.Domain.Missions;

namespace CityEscapeQuest.Application.Abstractions;

public interface IMissionRepository
{
    Task<IReadOnlyList<Mission>> BrowseAsync(int skip, int take, CancellationToken cancellationToken);
    Task<Mission?> GetAsync(Guid id, CancellationToken cancellationToken);
}