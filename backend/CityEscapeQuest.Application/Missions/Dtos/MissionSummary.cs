using CityEscapeQuest.Domain.Missions;

namespace CityEscapeQuest.Application.Missions.Dtos;

public sealed record MissionSummary(
    Guid Id,
    string Title,
    string ShortDescription,
    int RecommendedPlayers,
    int EstimatedDurationMinutes,
    decimal? Price,
    bool IsFree,
    string PreviewImageUrl)
{
    public static MissionSummary FromMission(Mission mission) => new(
        mission.Id,
        mission.Title,
        mission.ShortDescription,
        mission.RecommendedPlayers,
        mission.EstimatedDurationMinutes,
        mission.Price,
        mission.IsFree,
        mission.PreviewImageUrl);
}
