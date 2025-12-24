using MediatR;
using Microsoft.Extensions.Logging;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Connectors.Commands;
using SmartChargingManagement.Application.Mapper;
using SmartChargingManagement.Domain.Entities;

namespace SmartChargingManagement.Application.Features.Connectors.Handlers;

public class CreateConnectorCommandHandler(
    IConnectorRepository connectorRepository,
    IChargeStationRepository chargeStationRepository,
    IGroupRepository groupRepository,
    ILogger<CreateConnectorCommandHandler> logger)
    : IRequestHandler<CreateConnectorCommand, Response<ConnectorDto>>
{
    public async Task<Response<ConnectorDto>> Handle(CreateConnectorCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var chargeStation =
                await chargeStationRepository.GetByIdWithConnectorsAsync(request.ChargeStationId, cancellationToken);

            if (chargeStation == null)
                return new Response<ConnectorDto>(false,
                    null!,
                    $"Charge station with ID {request.ChargeStationId} was not found.");

            if (chargeStation.Connectors.Count >= 5)
                return new Response<ConnectorDto>(false,
                    null!,
                    "Cannot add connector: A charge station cannot have more than 5 connectors.");

            var nextAvailableId = GetNextAvailableConnectorId(chargeStation);
            if (nextAvailableId == null)
                return new Response<ConnectorDto>(false,
                    null!,
                    "Cannot add connector: No available connector ID slots (1-5) in this charge station.");

            if (!await IsGroupCapacitySufficientAsync(chargeStation.GroupId, request.MaxCurrentInAmps,
                    cancellationToken))
                return new Response<ConnectorDto>(false,
                    null!,
                    $"Cannot add connector: Group capacity would be less than the sum of all connector max currents.");

            var connector = await CreateAndAddConnectorAsync(nextAvailableId.Value, request, chargeStation, cancellationToken);

            return new Response<ConnectorDto>(true, connector.Map(), "Connector created successfully");

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating connector for charge station {ChargeStationId}", request.ChargeStationId);
            return new Response<ConnectorDto>(false,
                null!,
                $"An error occurred while creating the connector");
        }
    }

    private static int? GetNextAvailableConnectorId(ChargeStation chargeStation)
    {
        var usedIds = chargeStation.Connectors.Select(c => c.Id).ToHashSet();
        
        for (var id = 1; id <= 5; id++)
        {
            if (!usedIds.Contains(id))
                return id;
        }
        
        return null;
    }
    
    private async Task<bool> IsGroupCapacitySufficientAsync(Guid groupId, int connectorMaxCurrent, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdWithChargeStationsAsync(groupId, cancellationToken);
        if (group == null) return true;
        
        var totalMaxCurrent = group.GetTotalConnectorMaxCurrent() + connectorMaxCurrent;
        return group.CapacityInAmps >= totalMaxCurrent;
    }
    
    private async Task<Connector> CreateAndAddConnectorAsync(int connectorId, CreateConnectorCommand request,
        ChargeStation chargeStation,
        CancellationToken cancellationToken)
    {
        var connector = new Connector(connectorId, request.MaxCurrentInAmps, request.ChargeStationId);
        chargeStation.AddConnector(connector);
        return await connectorRepository.AddAsync(connector, cancellationToken);
    }
}

