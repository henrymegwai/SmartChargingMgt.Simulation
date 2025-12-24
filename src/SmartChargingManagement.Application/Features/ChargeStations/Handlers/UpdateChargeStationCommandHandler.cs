using MediatR;
using Microsoft.Extensions.Logging;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.ChargeStations.Commands;
using SmartChargingManagement.Application.Mapper;

namespace SmartChargingManagement.Application.Features.ChargeStations.Handlers;

public class UpdateChargeStationCommandHandler(IChargeStationRepository chargeStationRepository,
    ILogger<UpdateChargeStationCommandHandler> logger)
    : IRequestHandler<UpdateChargeStationCommand, Response<ChargeStationDto>>
{
    public async Task<Response<ChargeStationDto>> Handle(UpdateChargeStationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var chargeStation = await chargeStationRepository.GetByIdWithConnectorsAsync(request.Id, cancellationToken);

            if (chargeStation == null)
                return new Response<ChargeStationDto>(false, null!,
                    $"Charge station with ID {request.Id} was not found.");

            chargeStation.UpdateName(request.Name);

            await chargeStationRepository.UpdateAsync(chargeStation, cancellationToken);

            return new Response<ChargeStationDto>(true, chargeStation.Map(), "Charge station updated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating charge station with ID {ChargeStationId}", request.Id);
            return new Response<ChargeStationDto>(false, null!, "An error occurred while updating the charge station");
        }

    }
}