using MediatR;
using SmartChargingManagement.Application.Common.Models;

namespace SmartChargingManagement.Application.Features.ChargeStations.Queries;

public record GetAllChargeStationsQuery() : IRequest<Response<List<ChargeStationDto>>>;


