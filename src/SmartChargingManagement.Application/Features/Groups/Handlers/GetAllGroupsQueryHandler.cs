using MediatR;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Groups.Queries;
using SmartChargingManagement.Application.Mapper;

namespace SmartChargingManagement.Application.Features.Groups.Handlers;

public class GetAllGroupsQueryHandler(IGroupRepository groupRepository)
    : IRequestHandler<GetAllGroupsQuery, Response<List<GroupDto>>>
{
    public async Task<Response<List<GroupDto>>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        var groups = await groupRepository.GetAllAsync(cancellationToken);

        var groupDtos = groups.Select(g => g.Map()).ToList();

        return new Response<List<GroupDto>>(true, groupDtos, "Groups retrieved successfully");
    }
}


