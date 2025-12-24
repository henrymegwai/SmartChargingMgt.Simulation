using MediatR;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.ChargeStations.Queries;
using SmartChargingManagement.Application.Mapper;

namespace SmartChargingManagement.Application.Features.ChargeStations.Handlers;

public class GetChargeStationByIdQueryHandler(IChargeStationRepository chargeStationRepository)
    : IRequestHandler<GetChargeStationByIdQuery, Response<ChargeStationDto?>>
{
    public async Task<Response<ChargeStationDto?>> Handle(GetChargeStationByIdQuery request, CancellationToken cancellationToken)
    {
        var chargeStation = await chargeStationRepository.GetByIdWithConnectorsAsync(request.Id, cancellationToken);
        
        return chargeStation == null
            ? new Response<ChargeStationDto?>(true,
                null,
                "Charge station not found")
            : new Response<ChargeStationDto?>(true,
                chargeStation.Map(),
                "Charge station retrieved successfully");
    }
}


