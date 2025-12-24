using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Domain.Entities;

namespace SmartChargingManagement.Application.Mapper;

public static class Mapper
{
    public static GroupDto Map(this Group group)
    {
        return new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            CapacityInAmps = group.CapacityInAmps
        };
    }

    public static ChargeStationDto Map(this ChargeStation chargeStation)
    {
        return new ChargeStationDto
        {
            Id = chargeStation.Id,
            Name = chargeStation.Name,
            GroupId = chargeStation.GroupId,
            Connectors = chargeStation.Connectors.Select(c => c.Map()).ToList()
        };
    }

    public static ConnectorDto Map(this Connector connector)
    {
        return new ConnectorDto
        {
            Id = connector.Id,
            MaxCurrentInAmps = connector.MaxCurrentInAmps,
            ChargeStationId = connector.ChargeStationId
        };
    }
}