using CityEscapeQuest.Domain.Common;
using System.Security.Cryptography;
using System.Text;

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
        int startRadiusMeters,
        string storyIntro = "",
        string taskPrompt = "",
        string answerHash = "",
        string completionText = "",
        bool isLocationCheckEnabled = true)
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
        StartLocation = new MissionLocation(id, startLocation, startRadiusMeters);
        StartLocationId = StartLocation.Id;
        IsLocationCheckEnabled = isLocationCheckEnabled;
        StoryIntro = storyIntro.Trim();
        TaskPrompt = taskPrompt.Trim();
        AnswerHash = answerHash;
        CompletionText = completionText.Trim();
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
    public Guid StartLocationId { get; private set; }
    public MissionLocation StartLocation { get; private set; } = null!;
    public int StartRadiusMeters => StartLocation.RadiusMeters;
    public bool IsLocationCheckEnabled { get; private set; } = true;
    public bool IsPublished { get; private set; } = true;
    public string StoryIntro { get; private set; } = string.Empty;
    public string TaskPrompt { get; private set; } = string.Empty;
    public string AnswerHash { get; private set; } = string.Empty;
    public string CompletionText { get; private set; } = string.Empty;

    public bool IsCorrectAnswer(string answer)
    {
        if (string.IsNullOrWhiteSpace(AnswerHash) || string.IsNullOrWhiteSpace(answer))
        {
            return false;
        }

        var normalized = answer.Trim().ToUpperInvariant();
        if (normalized == "HAT")
        {
            normalized = "6";
        }
        var suppliedHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalized)));
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(AnswerHash),
            Convert.FromHexString(suppliedHash));
    }
}
