using MediatR;
using SmartChargingManagement.Application.Common.Models;

namespace SmartChargingManagement.Application.Features.ChargeStations.Commands;

public record CreateChargeStationCommand(string Name, Guid GroupId) : IRequest<Response<ChargeStationDto>>;


