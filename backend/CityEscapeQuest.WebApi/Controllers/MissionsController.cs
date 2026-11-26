using CityEscapeQuest.Application.Missions.Dtos;
using CityEscapeQuest.Application.Missions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityEscapeQuest.WebApi.Controllers;

[ApiController]
[Route("api/missions")]
public sealed class MissionsController(MissionCatalogService catalog) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<MissionSummary>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<MissionSummary>> Browse(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default) =>
        catalog.BrowseAsync(page, pageSize, cancellationToken);

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType<MissionDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MissionDetails>> Get(Guid id, CancellationToken cancellationToken)
    {
        var mission = await catalog.GetAsync(id, cancellationToken);
        return mission is null ? NotFound() : Ok(mission);
    }
}