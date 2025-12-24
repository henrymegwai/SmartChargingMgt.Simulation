using MediatR;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.ChargeStations.Queries;
using SmartChargingManagement.Application.Mapper;

namespace SmartChargingManagement.Application.Features.ChargeStations.Handlers;

public class GetAllChargeStationsQueryHandler(IChargeStationRepository chargeStationRepository)
    : IRequestHandler<GetAllChargeStationsQuery, Response<List<ChargeStationDto>>>
{
    public async Task<Response<List<ChargeStationDto>>> Handle(GetAllChargeStationsQuery request, CancellationToken cancellationToken)
    {
        var chargeStations = await chargeStationRepository.GetAllWithConnectorsAsync(cancellationToken);

        var chargeStationDtos = chargeStations.Select(cs => cs.Map()).ToList();

        return new Response<List<ChargeStationDto>>(true, chargeStationDtos, "Charge stations retrieved successfully");
    }
}

