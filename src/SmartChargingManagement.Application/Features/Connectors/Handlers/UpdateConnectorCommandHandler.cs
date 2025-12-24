using MediatR;
using Microsoft.Extensions.Logging;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Connectors.Commands;
using SmartChargingManagement.Application.Mapper;
using SmartChargingManagement.Domain.Entities;

namespace SmartChargingManagement.Application.Features.Connectors.Handlers;

public class UpdateConnectorCommandHandler(
    IConnectorRepository connectorRepository,
    IGroupRepository groupRepository,
    ILogger<UpdateConnectorCommandHandler> logger)
    : IRequestHandler<UpdateConnectorCommand, Response<ConnectorDto>>
{
    public async Task<Response<ConnectorDto>> Handle(UpdateConnectorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await HandleUpdateAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error updating connector with ID {ConnectorId} in charge station {ChargeStationId}",
                request.Id,
                request.ChargeStationId);
            
            return new Response<ConnectorDto>(false, null!, "An error occurred while updating the connector.");
        }
    }
    
    private async Task<Response<ConnectorDto>> HandleUpdateAsync(UpdateConnectorCommand request,
        CancellationToken cancellationToken)
    {
        var connector = await connectorRepository.GetByIdAndChargeStationIdAsync(request.Id,
            request.ChargeStationId,
            cancellationToken);

        if (connector == null)
            return new Response<ConnectorDto>(false,
                null!,
                $"Connector with ID {request.Id} in charge station {request.ChargeStationId} was not found.");

        var oldMaxCurrent = connector.MaxCurrentInAmps;
        connector.UpdateMaxCurrent(request.MaxCurrentInAmps);
        
        if (connector.ChargeStation == null!)
            return new Response<ConnectorDto>(false,
                null!,
                "Charge station information is missing.");

        var group = await groupRepository.GetByIdWithChargeStationsAsync(connector.ChargeStation.GroupId, cancellationToken);
        if (group != null && !IsGroupCapacitySufficient(group, oldMaxCurrent, request.MaxCurrentInAmps))
            return new Response<ConnectorDto>(false,
                null!,
                $"Cannot update connector: Group capacity ({group.CapacityInAmps} Amps) would be less than the sum of all connector max currents ({group.GetTotalConnectorMaxCurrent() - oldMaxCurrent + request.MaxCurrentInAmps} Amps).");

        await connectorRepository.UpdateAsync(connector, cancellationToken);

        return new Response<ConnectorDto>(true, connector.Map(), "Connector updated successfully");
    }
    
    private static bool IsGroupCapacitySufficient(Group group, int oldMaxCurrent, int newMaxCurrent)
    {
        var totalMaxCurrent = group.GetTotalConnectorMaxCurrent() - oldMaxCurrent + newMaxCurrent;
        return group.CapacityInAmps >= totalMaxCurrent;
    }
}

