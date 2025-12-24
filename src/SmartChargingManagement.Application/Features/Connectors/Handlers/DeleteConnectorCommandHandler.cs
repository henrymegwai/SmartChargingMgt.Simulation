using MediatR;
using Microsoft.Extensions.Logging;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Connectors.Commands;

namespace SmartChargingManagement.Application.Features.Connectors.Handlers;

public class DeleteConnectorCommandHandler(
    IConnectorRepository connectorRepository,
    IChargeStationRepository chargeStationRepository,
    ILogger<DeleteConnectorCommandHandler> logger)
    : IRequestHandler<DeleteConnectorCommand, Response<string>>
{
    public async Task<Response<string>> Handle(DeleteConnectorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var connector = await connectorRepository.GetByIdAndChargeStationIdAsync(request.Id, request.ChargeStationId, cancellationToken);

            if (connector == null)
                return new Response<string>(false, string.Empty, $"Connector with ID {request.Id} in charge station {request.ChargeStationId} was not found.");

            var chargeStation = await chargeStationRepository.GetByIdWithConnectorsAsync(request.ChargeStationId, cancellationToken);
        
            if (chargeStation == null)
                return new Response<string>(false, string.Empty, $"Charge station with ID {request.ChargeStationId} was not found.");
        
            chargeStation.RemoveConnector(connector);
        
            await connectorRepository.DeleteAsync(connector, cancellationToken);

            return new Response<string>(true, string.Empty, "Connector deleted successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting connector with ID {ConnectorId} from charge station {ChargeStationId}", request.Id, request.ChargeStationId);
            return new Response<string>(false, string.Empty, $"An error occurred while deleting the connector");
        }
    }
}

