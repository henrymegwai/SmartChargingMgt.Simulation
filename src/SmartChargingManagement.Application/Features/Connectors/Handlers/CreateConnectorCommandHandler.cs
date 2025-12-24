using MediatR;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Connectors.Commands;
using SmartChargingManagement.Application.Mapper;
using SmartChargingManagement.Domain.Entities;

namespace SmartChargingManagement.Application.Features.Connectors.Handlers;

public class CreateConnectorCommandHandler(
    IConnectorRepository connectorRepository,
    IChargeStationRepository chargeStationRepository,
    IGroupRepository groupRepository)
    : IRequestHandler<CreateConnectorCommand, Response<ConnectorDto>>
{
    public async Task<Response<ConnectorDto>> Handle(CreateConnectorCommand request, CancellationToken cancellationToken)
    {
        var chargeStation = await chargeStationRepository.GetByIdWithConnectorsAsync(request.ChargeStationId, cancellationToken);

        if (chargeStation == null)
            return new Response<ConnectorDto>(false,
                null!,
                $"Charge station with ID {request.ChargeStationId} was not found.");

        if (await ConnectorExistsAsync(request.Id, request.ChargeStationId, cancellationToken))
            return new Response<ConnectorDto>(false,
                null!,
                $"Connector with ID {request.Id} already exists in charge station {request.ChargeStationId}.");

        if (!await IsGroupCapacitySufficientAsync(chargeStation.GroupId, request.MaxCurrentInAmps, cancellationToken))
            return new Response<ConnectorDto>(false,
                null!,
                $"Cannot add connector: Group capacity would be less than the sum of all connector max currents.");
        
        var connector = await CreateAndAddConnectorAsync(request, chargeStation, cancellationToken);

        return new Response<ConnectorDto>(true, connector.Map(), "Connector created successfully");
    }
    
    private async Task<bool> ConnectorExistsAsync(int connectorId, Guid chargeStationId, CancellationToken cancellationToken)
    {
        var existingConnector = await connectorRepository.GetByIdAndChargeStationIdAsync(connectorId, chargeStationId, cancellationToken);
        return existingConnector != null;
    }
    
    private async Task<bool> IsGroupCapacitySufficientAsync(Guid groupId, int connectorMaxCurrent, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdWithChargeStationsAsync(groupId, cancellationToken);
        if (group == null) return true;
        
        var totalMaxCurrent = group.GetTotalConnectorMaxCurrent() + connectorMaxCurrent;
        return group.CapacityInAmps >= totalMaxCurrent;
    }
    
    private async Task<Connector> CreateAndAddConnectorAsync(CreateConnectorCommand request,
        ChargeStation chargeStation,
        CancellationToken cancellationToken)
    {
        var connector = new Connector(request.Id, request.MaxCurrentInAmps, request.ChargeStationId);
        chargeStation.AddConnector(connector);
        return await connectorRepository.AddAsync(connector, cancellationToken);
    }
}

