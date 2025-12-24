using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartChargingManagement.Api.Requests;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Groups.Commands;
using SmartChargingManagement.Application.Features.Groups.Queries;

namespace SmartChargingManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GroupsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(Response<GroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<GroupDto>),StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<GroupDto>>> GetAll()
    {
        var query = new GetAllGroupsQuery();
        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Response<GroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<GroupDto>),StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupDto>> GetById(Guid id)
    {
        var query = new GetGroupByIdQuery(id);
        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Response<GroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<GroupDto>),StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GroupDto>> Create([FromBody] CreateGroupRequest request)
    {
        var command = new CreateGroupCommand(request.Name, request.CapacityInAmps);
        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Response<GroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<GroupDto>),StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GroupDto>> Update(Guid id, [FromBody] UpdateGroupRequest request)
    {
        var command = new UpdateGroupCommand(id, request.Name, request.CapacityInAmps);
        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteGroupCommand(id);
        var response = await mediator.Send(command);
        return Ok(response);
    }
}

