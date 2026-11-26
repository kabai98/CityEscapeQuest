using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CityEscapeQuest.Application.Abstractions;
using CityEscapeQuest.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CityEscapeQuest.Infrastructure.Identity;

internal sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    AppDbContext dbContext,
    IOptions<JwtOptions> jwtOptions,
    TimeProvider timeProvider) : IIdentityService
{
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public async Task<AuthTokens> RegisterAsync(
        string email,
        string password,
        string displayName,
        CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = email.Trim(),
            Email = email.Trim(),
            DisplayName = displayName.Trim()
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new IdentityOperationException(result.Errors.Select(error => error.Description));
        }

        await userManager.AddToRoleAsync(user, Roles.User);
        return await IssueTokenPairAsync(user, Guid.NewGuid(), cancellationToken);
    }

    public async Task<AuthTokens> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!await userManager.CheckPasswordAsync(user, password))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        return await IssueTokenPairAsync(user, Guid.NewGuid(), cancellationToken);
    }

    public async Task<AuthTokens> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var tokenHash = Hash(refreshToken);
        var current = await dbContext.RefreshTokenSessions
            .Include(token => token.User)
            .SingleOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken)
            ?? throw new UnauthorizedAccessException("Refresh token is invalid.");

        if (!current.IsActive(now))
        {
            await RevokeFamilyAsync(current.FamilyId, now, cancellationToken);
            throw new UnauthorizedAccessException("Refresh token reuse was detected.");
        }

        current.RevokedAtUtc = now;
        var tokens = await IssueTokenPairAsync(current.User, current.FamilyId, cancellationToken);
        current.ReplacedByTokenHash = Hash(tokens.RefreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return tokens;
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var tokenHash = Hash(refreshToken);
        var session = await dbContext.RefreshTokenSessions
            .SingleOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

        if (session is not null && session.RevokedAtUtc is null)
        {
            session.RevokedAtUtc = timeProvider.GetUtcNow();
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<AuthTokens> IssueTokenPairAsync(
        ApplicationUser user,
        Guid familyId,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var accessExpiry = now.AddMinutes(_jwt.AccessTokenMinutes);
        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.DisplayName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey)),
            SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: accessExpiry.UtcDateTime,
            signingCredentials: signingCredentials);

        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        dbContext.RefreshTokenSessions.Add(new RefreshTokenSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FamilyId = familyId,
            TokenHash = Hash(refreshToken),
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(_jwt.RefreshTokenDays)
        });
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthTokens(
            new JwtSecurityTokenHandler().WriteToken(jwt),
            refreshToken,
            accessExpiry);
    }

    private async Task RevokeFamilyAsync(Guid familyId, DateTimeOffset revokedAtUtc, CancellationToken cancellationToken)
    {
        var activeTokens = await dbContext.RefreshTokenSessions
            .Where(token => token.FamilyId == familyId && token.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);
        foreach (var token in activeTokens)
        {
            token.RevokedAtUtc = revokedAtUtc;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string Hash(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}

public sealed class IdentityOperationException(IEnumerable<string> errors)
    : Exception(string.Join(" ", errors));

public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Guest = "Guest";

    public static readonly string[] All = [Admin, User, Guest];
}