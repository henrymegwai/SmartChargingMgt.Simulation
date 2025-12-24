using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartChargingManagement.Api.Requests;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.ChargeStations.Commands;
using SmartChargingManagement.Application.Features.ChargeStations.Queries;

namespace SmartChargingManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ChargeStationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ChargeStationDto>>> GetAll()
    {
        var query = new GetAllChargeStationsQuery();
        var response = await mediator.Send(query);
        return Ok(response.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ChargeStationDto>> GetById(Guid id)
    {
        var query = new GetChargeStationByIdQuery(id);
        var response = await mediator.Send(query);

        if (response.Data == null)
            return NotFound();

        return Ok(response.Data);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ChargeStationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ChargeStationDto>> Create([FromBody] CreateChargeStationRequest request)
    {
        var command = new CreateChargeStationCommand(request.Name, request.GroupId);
        var response = await mediator.Send(command);
        
        if (!response.Status || response.Data == null)
            return BadRequest(response.Message);
            
        return CreatedAtAction(nameof(GetById), new { id = response.Data.Id }, response.Data);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ChargeStationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChargeStationDto>> Update(Guid id, [FromBody] UpdateChargeStationRequest request)
    {
        var command = new UpdateChargeStationCommand(id, request.Name);
        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteChargeStationCommand(id);
        var response = await mediator.Send(command);
        return Ok(response);
    }
}

