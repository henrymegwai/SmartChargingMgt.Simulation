using MediatR;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Groups.Queries;
using SmartChargingManagement.Application.Mapper;

namespace SmartChargingManagement.Application.Features.Groups.Handlers;

public class GetGroupByIdQueryHandler(IGroupRepository groupRepository)
    : IRequestHandler<GetGroupByIdQuery, Response<GroupDto?>>
{
    public async Task<Response<GroupDto?>> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.Id, cancellationToken);

        return group == null
            ? new Response<GroupDto?>(true,
                null,
                "Group not found")
            : new Response<GroupDto?>(true,
                group.Map(),
                "Group retrieved successfully");
    }
}


