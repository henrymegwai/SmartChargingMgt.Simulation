using MediatR;
using SmartChargingManagement.Application.Common.Models;

namespace SmartChargingManagement.Application.Features.ChargeStations.Commands;

public record UpdateChargeStationCommand(Guid Id, string Name) : IRequest<Response<ChargeStationDto>>;


