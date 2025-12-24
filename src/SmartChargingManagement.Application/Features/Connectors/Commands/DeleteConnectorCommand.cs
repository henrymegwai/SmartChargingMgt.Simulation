using MediatR;
using SmartChargingManagement.Application.Common.Models;

namespace SmartChargingManagement.Application.Features.Connectors.Commands;

public record DeleteConnectorCommand(int Id, Guid ChargeStationId) : IRequest<Response<string>>;


