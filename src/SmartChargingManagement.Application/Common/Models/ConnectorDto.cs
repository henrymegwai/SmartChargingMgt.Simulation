namespace SmartChargingManagement.Application.Common.Models;

public record ConnectorDto
{
    public int Id { get; init; }
    public int MaxCurrentInAmps { get; init; }
    public Guid ChargeStationId { get; init; }
}


