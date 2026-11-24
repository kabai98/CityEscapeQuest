using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CityEscapeQuest.Application.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CityEscapeQuest.Infrastructure.Identity;

internal sealed class GuestAccessTokenService(
    IOptions<JwtOptions> jwtOptions,
    TimeProvider timeProvider) : IGuestAccessTokenService
{
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public GuestAccessToken Issue(Guid sessionId, Guid participantId, string displayName)
    {
        var now = timeProvider.GetUtcNow();
        var expiresAt = now.AddHours(4);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey)),
            SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, $"guest:{participantId}"),
            new Claim(ClaimTypes.NameIdentifier, $"guest:{participantId}"),
            new Claim(ClaimTypes.Name, displayName),
            new Claim(ClaimTypes.Role, Roles.Guest),
            new Claim("game_session_id", sessionId.ToString()),
            new Claim("participant_id", participantId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var jwt = new JwtSecurityToken(
            _jwt.Issuer,
            _jwt.Audience,
            claims,
            now.UtcDateTime,
            expiresAt.UtcDateTime,
            credentials);
        return new GuestAccessToken(new JwtSecurityTokenHandler().WriteToken(jwt), expiresAt);
    }
}
