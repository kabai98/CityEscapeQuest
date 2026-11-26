using CityEscapeQuest.Domain.Missions;

namespace CityEscapeQuest.Application.Missions.Dtos;

public sealed record MissionDetails(
    Guid Id,
    string Title,
    string ShortDescription,
    int MinimumPlayers,
    int RecommendedPlayers,
    int EstimatedDurationMinutes,
    decimal? Price,
    bool IsFree,
    string PreviewImageUrl,
    double StartLatitude,
    double StartLongitude,
    int StartRadiusMeters,
    bool IsLocationCheckEnabled)
{
    public static MissionDetails FromMission(Mission mission) => new(
        mission.Id,
        mission.Title,
        mission.ShortDescription,
        mission.MinimumPlayers,
        mission.RecommendedPlayers,
        mission.EstimatedDurationMinutes,
        mission.Price,
        mission.IsFree,
        mission.PreviewImageUrl,
        mission.StartLocation.Latitude,
        mission.StartLocation.Longitude,
        mission.StartRadiusMeters,
        mission.IsLocationCheckEnabled);
}
