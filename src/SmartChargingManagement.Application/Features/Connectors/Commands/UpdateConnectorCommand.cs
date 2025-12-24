using MediatR;
using SmartChargingManagement.Application.Common.Models;

namespace SmartChargingManagement.Application.Features.Connectors.Commands;

public record UpdateConnectorCommand(int Id, Guid ChargeStationId, int MaxCurrentInAmps) : IRequest<Response<ConnectorDto>>;


