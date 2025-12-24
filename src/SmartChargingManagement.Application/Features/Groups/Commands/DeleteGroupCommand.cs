using MediatR;
using SmartChargingManagement.Application.Common.Models;

namespace SmartChargingManagement.Application.Features.Groups.Commands;

public record DeleteGroupCommand(Guid Id) : IRequest<Response<string>>;


