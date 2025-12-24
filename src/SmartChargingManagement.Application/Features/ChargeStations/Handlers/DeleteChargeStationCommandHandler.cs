using MediatR;
using Microsoft.Extensions.Logging;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.ChargeStations.Commands;

namespace SmartChargingManagement.Application.Features.ChargeStations.Handlers;

public class DeleteChargeStationCommandHandler(IChargeStationRepository chargeStationRepository,
    ILogger<DeleteChargeStationCommandHandler> logger)
    : IRequestHandler<DeleteChargeStationCommand, Response<string>>
{
    public async Task<Response<string>> Handle(DeleteChargeStationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var chargeStation = await chargeStationRepository.GetByIdAsync(request.Id, cancellationToken);

            if (chargeStation == null)
                return new Response<string>(false, string.Empty, $"Charge station with ID {request.Id} was not found.");
        
            await chargeStationRepository.DeleteAsync(chargeStation, cancellationToken);

            return new Response<string>(true, string.Empty, "Charge station deleted successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting charge station with ID {ChargeStationId}", request.Id);
            return new Response<string>(false, string.Empty, $"An error occurred while deleting the charge station");
        }
    }
}


