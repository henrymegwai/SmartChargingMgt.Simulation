using MediatR;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.ChargeStations.Commands;
using SmartChargingManagement.Domain.Entities;
using SmartChargingManagement.Application.Mapper;

namespace SmartChargingManagement.Application.Features.ChargeStations.Handlers;

public class CreateChargeStationCommandHandler(
    IChargeStationRepository chargeStationRepository,
    IGroupRepository groupRepository)
    : IRequestHandler<CreateChargeStationCommand, Response<ChargeStationDto>>
{
    public async Task<Response<ChargeStationDto>> Handle(CreateChargeStationCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group == null)
            return new Response<ChargeStationDto>(false, null!, $"Group with ID {request.GroupId} was not found.");

        var chargeStation = new ChargeStation(Guid.NewGuid(), request.Name, request.GroupId);

        chargeStation = await chargeStationRepository.AddAsync(chargeStation, cancellationToken);
        
        return new Response<ChargeStationDto>(true, chargeStation.Map(),"Charge station created successfully");
    }
}


