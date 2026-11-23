using CityEscapeQuest.Domain.Common;

namespace CityEscapeQuest.Domain.Missions;

public sealed class Mission
{
    private Mission()
    {
    }

    public Mission(
        Guid id,
        string title,
        string shortDescription,
        int minimumPlayers,
        int recommendedPlayers,
        int estimatedDurationMinutes,
        decimal? price,
        string previewImageUrl,
        GeoPoint startLocation,
        int startRadiusMeters)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Mission title is required.");
        }

        if (minimumPlayers < 1 || recommendedPlayers < minimumPlayers)
        {
            throw new DomainException("Player limits are invalid.");
        }

        if (estimatedDurationMinutes < 1 || startRadiusMeters < 1 || price < 0)
        {
            throw new DomainException("Mission duration, radius, and price must be valid.");
        }

        Id = id;
        Title = title.Trim();
        ShortDescription = shortDescription.Trim();
        MinimumPlayers = minimumPlayers;
        RecommendedPlayers = recommendedPlayers;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        Price = price;
        PreviewImageUrl = previewImageUrl.Trim();
        StartLocation = startLocation;
        StartRadiusMeters = startRadiusMeters;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string ShortDescription { get; private set; } = string.Empty;
    public int MinimumPlayers { get; private set; }
    public int RecommendedPlayers { get; private set; }
    public int EstimatedDurationMinutes { get; private set; }
    public decimal? Price { get; private set; }
    public bool IsFree => Price is null or 0;
    public string PreviewImageUrl { get; private set; } = string.Empty;
    public GeoPoint StartLocation { get; private set; }
    public int StartRadiusMeters { get; private set; }
    public bool IsPublished { get; private set; } = true;
}