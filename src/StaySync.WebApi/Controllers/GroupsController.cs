using Microsoft.AspNetCore.Mvc;
using StaySync.Application.Interfaces;
using StaySync.Application.Features.Groups.Queries.GetGroupRooms;

namespace StaySync.WebApi.Controllers;

[ApiController]
[Route("api/v1/groups")]
public sealed class GroupsController(IDispatcher dispatcher) : ControllerBase
{
    /// <summary>All rooms in a travel group (for hotel-local today)</summary>
    [HttpGet("{groupId}/rooms")]
    [ProducesResponseType(typeof(GroupRoomsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<GroupRoomsDto>> GetRooms([FromRoute] string groupId, CancellationToken ct)
    {
        var dto = await dispatcher.Query(new GetGroupRoomsQuery(groupId), ct);
        return Ok(dto);
    }
}
