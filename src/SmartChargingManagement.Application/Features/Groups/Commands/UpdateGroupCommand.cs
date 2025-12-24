using MediatR;
using SmartChargingManagement.Application.Common.Models;

namespace SmartChargingManagement.Application.Features.Groups.Commands;

public record UpdateGroupCommand(Guid Id, string? Name, int? CapacityInAmps) : IRequest<Response<GroupDto>>;


