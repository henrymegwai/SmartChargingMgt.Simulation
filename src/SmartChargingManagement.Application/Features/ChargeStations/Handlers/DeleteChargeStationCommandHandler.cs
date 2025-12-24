using MediatR;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.ChargeStations.Commands;

namespace SmartChargingManagement.Application.Features.ChargeStations.Handlers;

public class DeleteChargeStationCommandHandler(IChargeStationRepository chargeStationRepository)
    : IRequestHandler<DeleteChargeStationCommand, Response<string>>
{
    public async Task<Response<string>> Handle(DeleteChargeStationCommand request, CancellationToken cancellationToken)
    {
        var chargeStation = await chargeStationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (chargeStation == null)
            return new Response<string>(false, string.Empty, $"Charge station with ID {request.Id} was not found.");

        // Cascading delete: Connectors will be automatically deleted
        // due to EF Core cascade delete configuration
        await chargeStationRepository.DeleteAsync(chargeStation, cancellationToken);

        return new Response<string>(true, string.Empty, "Charge station deleted successfully");
    }
}


