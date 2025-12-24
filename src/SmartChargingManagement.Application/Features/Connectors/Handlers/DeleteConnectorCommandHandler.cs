using MediatR;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Connectors.Commands;

namespace SmartChargingManagement.Application.Features.Connectors.Handlers;

public class DeleteConnectorCommandHandler(
    IConnectorRepository connectorRepository,
    IChargeStationRepository chargeStationRepository)
    : IRequestHandler<DeleteConnectorCommand, Response<string>>
{
    public async Task<Response<string>> Handle(DeleteConnectorCommand request, CancellationToken cancellationToken)
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
}

