using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartChargingManagement.Api.Requests;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Connectors.Commands;
using SmartChargingManagement.Application.Features.Connectors.Queries;

namespace SmartChargingManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ConnectorsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{chargeStationId:guid}/{id:int}")]
    [ProducesResponseType(typeof(Response<ConnectorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<ConnectorDto>),StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConnectorDto>> GetById(Guid chargeStationId, int id)
    {
        var query = new GetConnectorByIdQuery(id, chargeStationId);
        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Response<ConnectorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ConnectorDto>> Create([FromBody] CreateConnectorRequest request)
    {
        var command = new CreateConnectorCommand(request.Id, request.MaxCurrentInAmps, request.ChargeStationId);
        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpPut("{chargeStationId:guid}/{id:int}")]
    [ProducesResponseType(typeof(Response<ConnectorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<ConnectorDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ConnectorDto>> Update(Guid chargeStationId,
        int id,
        [FromBody] UpdateConnectorRequest request)
    {
        var command = new UpdateConnectorCommand(id, chargeStationId, request.MaxCurrentInAmps);
        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{chargeStationId:guid}/{id:int}")]
    [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid chargeStationId, int id)
    {
        var command = new DeleteConnectorCommand(id, chargeStationId);
        var response = await mediator.Send(command);
        return Ok(response);
    }
}

