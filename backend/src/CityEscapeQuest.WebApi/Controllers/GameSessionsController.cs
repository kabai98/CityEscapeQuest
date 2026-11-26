using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CityEscapeQuest.Application.GameSessions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CityEscapeQuest.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/game-sessions")]
public sealed class GameSessionsController(
    GameSessionService gameSessions,
    IValidator<ReportLocationRequest> locationValidator) : ControllerBase
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
    [ProducesResponseType<GamePlayState>(StatusCodes.Status200OK)]
    public Task<GamePlayState> Start(
        Guid id,
        CancellationToken cancellationToken) =>
        gameSessions.StartAsync(id, GetUserId(), cancellationToken);

    [HttpPost("{id:guid}/location")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ReportLocation(
        Guid id,
        ReportLocationRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await locationValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));
        }

        await gameSessions.ReportLocationAsync(
            id,
            GetUserId(),
            GetGuestParticipantId(),
            new LocationInput(request.Latitude, request.Longitude, request.AccuracyMeters, request.MeasuredAtUtc),
            cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}/state")]
    [ProducesResponseType<GamePlayState>(StatusCodes.Status200OK)]
    public Task<GamePlayState> GetState(Guid id, CancellationToken cancellationToken) =>
        gameSessions.GetGameStateAsync(id, GetUserId(), GetGuestParticipantId(), cancellationToken);

    [HttpPost("{id:guid}/answer")]
    [ProducesResponseType<GamePlayState>(StatusCodes.Status200OK)]
    public Task<GamePlayState> SubmitAnswer(
        Guid id,
        SubmitAnswerRequest request,
        CancellationToken cancellationToken) =>
        gameSessions.SubmitAnswerAsync(id, GetUserId(), GetGuestParticipantId(), request.Answer, cancellationToken);

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? throw new UnauthorizedAccessException("User identifier is missing.");

    private Guid? GetGuestParticipantId() =>
        Guid.TryParse(User.FindFirstValue("participant_id"), out var participantId) ? participantId : null;
}

public sealed record CreateGameSessionRequest(Guid MissionId);
public sealed record JoinGameSessionRequest(
    [Required, RegularExpression("^[0-9]{6}$")] string JoinCode,
    [Required, MaxLength(40)] string DisplayName);

public sealed record ReportLocationRequest(
    double Latitude,
    double Longitude,
    double AccuracyMeters,
    DateTimeOffset MeasuredAtUtc);
public sealed record SubmitAnswerRequest([Required, MaxLength(100)] string Answer);
