using MediatR;
using SmartChargingManagement.Application.Common.Models;

namespace SmartChargingManagement.Application.Features.Connectors.Commands;

public record CreateConnectorCommand(int MaxCurrentInAmps, Guid ChargeStationId) : IRequest<Response<ConnectorDto>>;