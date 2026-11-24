using System.ComponentModel.DataAnnotations;
using CityEscapeQuest.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CityEscapeQuest.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
[EnableRateLimiting("authentication")]
public sealed class AuthController(IIdentityService identityService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<AuthTokens>(StatusCodes.Status200OK)]
    public Task<AuthTokens> Register(RegisterRequest request, CancellationToken cancellationToken) =>
        identityService.RegisterAsync(request.Email, request.Password, request.DisplayName, cancellationToken);

    [HttpPost("login")]
    [ProducesResponseType<AuthTokens>(StatusCodes.Status200OK)]
    public Task<AuthTokens> Login(LoginRequest request, CancellationToken cancellationToken) =>
        identityService.LoginAsync(request.Email, request.Password, cancellationToken);

    [HttpPost("refresh")]
    [ProducesResponseType<AuthTokens>(StatusCodes.Status200OK)]
    public Task<AuthTokens> Refresh(RefreshRequest request, CancellationToken cancellationToken) =>
        identityService.RefreshAsync(request.RefreshToken, cancellationToken);

    [HttpPost("revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Revoke(RefreshRequest request, CancellationToken cancellationToken)
    {
        await identityService.RevokeAsync(request.RefreshToken, cancellationToken);
        return NoContent();
    }
}

public sealed record RegisterRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(10)] string Password,
    [Required, MaxLength(80)] string DisplayName);

public sealed record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public sealed record RefreshRequest([Required] string RefreshToken);
