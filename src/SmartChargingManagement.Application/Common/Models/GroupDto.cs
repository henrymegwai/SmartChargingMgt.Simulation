namespace SmartChargingManagement.Application.Common.Models;

public record GroupDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int CapacityInAmps { get; init; }
}


