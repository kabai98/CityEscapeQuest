namespace CityEscapeQuest.Application.Abstractions;

public interface IIdentityService
{
    Task<AuthTokens> RegisterAsync(string email, string password, string displayName, CancellationToken cancellationToken);
    Task<AuthTokens> LoginAsync(string email, string password, CancellationToken cancellationToken);
    Task<AuthTokens> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken);
}

public sealed record AuthTokens(string AccessToken, string RefreshToken, DateTimeOffset AccessTokenExpiresAtUtc);