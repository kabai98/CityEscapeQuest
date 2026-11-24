using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CityEscapeQuest.Application.GameSessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CityEscapeQuest.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/game-sessions")]
public sealed class GameSessionsController(GameSessionService gameSessions) : ControllerBase
{
    [AllowAnonymous]
    [EnableRateLimiting("guest-join")]
    [HttpPost("join")]
    [ProducesResponseType<JoinedGameSession>(StatusCodes.Status200OK)]
    public Task<JoinedGameSession> Join(
        JoinGameSessionRequest request,
        CancellationToken cancellationToken) =>
        gameSessions.JoinAsync(
            request.JoinCode,
            request.DisplayName,
            User.Identity?.IsAuthenticated == true ? GetUserId() : null,
            cancellationToken);

    [HttpGet("{id:guid}")]
    [ProducesResponseType<GameSessionLobby>(StatusCodes.Status200OK)]
    public Task<GameSessionLobby> GetLobby(Guid id, CancellationToken cancellationToken)
    {
        var participantClaim = User.FindFirstValue("participant_id");
        var guestParticipantId = Guid.TryParse(participantClaim, out var parsed) ? parsed : (Guid?)null;
        return gameSessions.GetLobbyAsync(id, GetUserId(), guestParticipantId, cancellationToken);
    }

    [HttpPost]
    [ProducesResponseType<CreatedGameSession>(StatusCodes.Status201Created)]
    public async Task<ActionResult<CreatedGameSession>> Create(
        CreateGameSessionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await gameSessions.CreateAsync(
            request.MissionId,
            GetUserId(),
            User.Identity?.Name ?? "Player",
            cancellationToken);
        return Created($"/api/game-sessions/{result.SessionId}", result);
    }

    [HttpPost("{id:guid}/start")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Start(
        Guid id,
        StartGameSessionRequest request,
        CancellationToken cancellationToken)
    {
        await gameSessions.StartAsync(id, GetUserId(), request.Locations, cancellationToken);
        return NoContent();
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? throw new UnauthorizedAccessException("User identifier is missing.");
}

public sealed record CreateGameSessionRequest(Guid MissionId);
public sealed record JoinGameSessionRequest(
    [Required, RegularExpression("^[0-9]{6}$")] string JoinCode,
    [Required, MaxLength(40)] string DisplayName);

public sealed record StartGameSessionRequest(
    [Required, MinLength(1)] IReadOnlyCollection<LocationInput> Locations);
