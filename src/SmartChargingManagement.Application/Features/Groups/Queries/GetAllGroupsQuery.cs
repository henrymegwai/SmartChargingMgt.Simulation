using MediatR;
using SmartChargingManagement.Application.Common.Models;

namespace SmartChargingManagement.Application.Features.Groups.Queries;

public record GetAllGroupsQuery() : IRequest<Response<List<GroupDto>>>;


