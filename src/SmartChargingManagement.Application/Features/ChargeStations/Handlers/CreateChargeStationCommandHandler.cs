using MediatR;
using Microsoft.Extensions.Logging;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.ChargeStations.Commands;
using SmartChargingManagement.Domain.Entities;
using SmartChargingManagement.Application.Mapper;

namespace SmartChargingManagement.Application.Features.ChargeStations.Handlers;

public class CreateChargeStationCommandHandler(
    IChargeStationRepository chargeStationRepository,
    IGroupRepository groupRepository,
    ILogger<CreateChargeStationCommandHandler> logger)
    : IRequestHandler<CreateChargeStationCommand, Response<ChargeStationDto>>
{
    public async Task<Response<ChargeStationDto>> Handle(CreateChargeStationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
            if (group == null)
                return new Response<ChargeStationDto>(false, null!, $"Group with ID {request.GroupId} was not found.");

            var existingChargeStation = await chargeStationRepository.ExistsInGroupAsync(request.Name, request.GroupId, cancellationToken);
            if (existingChargeStation)
                return new Response<ChargeStationDto>(false, null!, $"A charge station with the name '{request.Name}' already exists in the specified group.");
            
            
            var chargeStation = new ChargeStation(Guid.NewGuid(), request.Name, request.GroupId);

            chargeStation = await chargeStationRepository.AddAsync(chargeStation, cancellationToken);

            return new Response<ChargeStationDto>(true, chargeStation.Map(), "Charge station created successfully");

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating charge station");
            return new Response<ChargeStationDto>(false, null!, "An error occurred while creating the charge station.");
        }
    }
}


