using MediatR;
using SmartChargingManagement.Application.Common.Models;

namespace SmartChargingManagement.Application.Features.Groups.Queries;

public record GetGroupByIdQuery(Guid Id) : IRequest<Response<GroupDto?>>;


