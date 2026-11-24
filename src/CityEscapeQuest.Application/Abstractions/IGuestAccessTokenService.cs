namespace CityEscapeQuest.Application.Abstractions;

public interface IGuestAccessTokenService
{
    GuestAccessToken Issue(Guid sessionId, Guid participantId, string displayName);
}

public sealed record GuestAccessToken(string AccessToken, DateTimeOffset ExpiresAtUtc);
