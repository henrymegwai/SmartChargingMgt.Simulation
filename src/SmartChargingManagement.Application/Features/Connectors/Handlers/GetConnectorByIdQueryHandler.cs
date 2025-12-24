using MediatR;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Connectors.Queries;
using SmartChargingManagement.Application.Mapper;

namespace SmartChargingManagement.Application.Features.Connectors.Handlers;

public class GetConnectorByIdQueryHandler(IConnectorRepository connectorRepository)
    : IRequestHandler<GetConnectorByIdQuery, Response<ConnectorDto?>>
{
    public async Task<Response<ConnectorDto?>> Handle(GetConnectorByIdQuery request, CancellationToken cancellationToken)
    {
        var connector = await connectorRepository.GetByIdAndChargeStationIdAsync(request.Id, request.ChargeStationId, cancellationToken);

        return connector == null
            ? new Response<ConnectorDto?>(true,
                null,
                "Connector not found")
            : new Response<ConnectorDto?>(true,
                connector.Map(),
                "Connector retrieved successfully");
    }
}


