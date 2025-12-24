using MediatR;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.ChargeStations.Commands;
using SmartChargingManagement.Application.Mapper;

namespace SmartChargingManagement.Application.Features.ChargeStations.Handlers;

public class UpdateChargeStationCommandHandler(IChargeStationRepository chargeStationRepository)
    : IRequestHandler<UpdateChargeStationCommand, Response<ChargeStationDto>>
{
    public async Task<Response<ChargeStationDto>> Handle(UpdateChargeStationCommand request, CancellationToken cancellationToken)
    {
        var chargeStation = await chargeStationRepository.GetByIdWithConnectorsAsync(request.Id, cancellationToken);

        if (chargeStation == null)
            return new Response<ChargeStationDto>(false, null!, $"Charge station with ID {request.Id} was not found.");

        if (request.Name != null)
            chargeStation.UpdateName(request.Name);

        await chargeStationRepository.UpdateAsync(chargeStation, cancellationToken);

        return new Response<ChargeStationDto>(true, chargeStation.Map(), "Charge station updated successfully");
    }
}