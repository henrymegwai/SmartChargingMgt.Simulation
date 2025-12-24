using MediatR;
using SmartChargingManagement.Application.Common.Models;

namespace SmartChargingManagement.Application.Features.ChargeStations.Commands;

public record DeleteChargeStationCommand(Guid Id) : IRequest<Response<string>>;


