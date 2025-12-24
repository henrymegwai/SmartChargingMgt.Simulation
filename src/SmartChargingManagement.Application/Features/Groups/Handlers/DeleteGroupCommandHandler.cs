using MediatR;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Groups.Commands;

namespace SmartChargingManagement.Application.Features.Groups.Handlers;

public class DeleteGroupCommandHandler(IGroupRepository groupRepository) : IRequestHandler<DeleteGroupCommand, Response<string>>
{
    public async Task<Response<string>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.Id, cancellationToken);

        if (group == null)
            return new Response<string>(false, string.Empty, $"Group with ID {request.Id} was not found.");
        
        await groupRepository.DeleteAsync(group, cancellationToken);

        return new Response<string>(true, string.Empty, "Group deleted successfully");
    }
}