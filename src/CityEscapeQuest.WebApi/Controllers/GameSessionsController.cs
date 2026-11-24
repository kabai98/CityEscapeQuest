using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CityEscapeQuest.Application.GameSessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityEscapeQuest.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/game-sessions")]
public sealed class GameSessionsController(GameSessionService gameSessions) : ControllerBase
{
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

public sealed record StartGameSessionRequest(
    [property: Required, MinLength(1)] IReadOnlyCollection<LocationInput> Locations);