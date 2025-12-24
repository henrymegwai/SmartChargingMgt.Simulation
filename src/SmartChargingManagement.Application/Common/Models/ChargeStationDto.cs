namespace SmartChargingManagement.Application.Common.Models;

public record ChargeStationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid GroupId { get; init; }
    public List<ConnectorDto> Connectors { get; init; } = new();
}


