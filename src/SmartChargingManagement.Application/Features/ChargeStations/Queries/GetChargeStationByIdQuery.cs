using MediatR;
using SmartChargingManagement.Application.Common.Models;

namespace SmartChargingManagement.Application.Features.ChargeStations.Queries;

public record GetChargeStationByIdQuery(Guid Id) : IRequest<Response<ChargeStationDto?>>;


