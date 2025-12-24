using MediatR;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Groups.Commands;
using SmartChargingManagement.Application.Mapper;
using SmartChargingManagement.Domain.Entities;

namespace SmartChargingManagement.Application.Features.Groups.Handlers;

public class CreateGroupCommandHandler(IGroupRepository groupRepository)
    : IRequestHandler<CreateGroupCommand, Response<GroupDto>>
{
    public async Task<Response<GroupDto>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = new Group(Guid.NewGuid(), request.Name, request.CapacityInAmps);

        group = await groupRepository.AddAsync(group, cancellationToken);

        return new Response<GroupDto>(true, group.Map(), "Group created successfully");
    }
}