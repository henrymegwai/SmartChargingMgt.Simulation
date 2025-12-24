using MediatR;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Groups.Commands;
using SmartChargingManagement.Application.Mapper;

namespace SmartChargingManagement.Application.Features.Groups.Handlers;

public class UpdateGroupCommandHandler(IGroupRepository groupRepository)
    : IRequestHandler<UpdateGroupCommand, Response<GroupDto>>
{
    public async Task<Response<GroupDto>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdWithChargeStationsAsync(request.Id, cancellationToken);

        if (group == null)
            return new Response<GroupDto>(false, null!, $"Group with ID {request.Id} was not found.");
        
        if (request.Name != null)
            group.UpdateName(request.Name);

        if (request.CapacityInAmps.HasValue)
            group.UpdateCapacity(request.CapacityInAmps.Value);

        await groupRepository.UpdateAsync(group, cancellationToken);

        return new Response<GroupDto>(true, group.Map(), "Group updated successfully");
    }
}


