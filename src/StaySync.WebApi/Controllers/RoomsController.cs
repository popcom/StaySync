using Microsoft.AspNetCore.Mvc;
using StaySync.Application.Interfaces;
using StaySync.Application.Features.Rooms.Queries.GetRoomsToBeOccupiedToday;
using StaySync.Application.Features.Rooms.Queries.GetRoomByCode;
using StaySync.Application.Features.Rooms.Commands.MoveTraveller;

namespace StaySync.WebApi.Controllers;

[ApiController]
[Route("api/v1/rooms")]
public sealed class RoomsController(IDispatcher dispatcher) : ControllerBase
{
    /// <summary>Rooms to be occupied today (hotel-local date)</summary>
    [HttpGet("occupancy/today")]
    [ProducesResponseType(typeof(TodayOccupancyDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TodayOccupancyDto>> GetToday(CancellationToken ct)
    {
        var dto = await dispatcher.Query(new GetRoomsToBeOccupiedTodayQuery(), ct);
        return Ok(dto);
    }

    /// <summary>Get individual room with today's occupancy</summary>
    [HttpGet("{roomCode}")]
    [ProducesResponseType(typeof(RoomDetailsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RoomDetailsDto>> GetByCode([FromRoute] string roomCode, CancellationToken ct)
    {
        var dto = await dispatcher.Query(new GetRoomByCodeQuery(roomCode), ct);
        return Ok(dto);
    }

    /// <summary>Move a traveller from one room to another for the given date</summary>
    [HttpPost("move")]
    [ProducesResponseType(typeof(MoveTravellerResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MoveTravellerResult>> Move([FromBody] MoveTravellerRequest body, CancellationToken ct)
    {
        var result = await dispatcher.Send(new MoveTravellerCommand(body), ct);
        return Ok(result);
    }
}
