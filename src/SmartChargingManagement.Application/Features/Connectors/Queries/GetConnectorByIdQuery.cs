using MediatR;
using SmartChargingManagement.Application.Common.Models;

namespace SmartChargingManagement.Application.Features.Connectors.Queries;

public record GetConnectorByIdQuery(int Id, Guid ChargeStationId) : IRequest<Response<ConnectorDto?>>;


