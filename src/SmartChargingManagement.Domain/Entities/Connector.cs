namespace SmartChargingManagement.Domain.Entities;

public class Connector
{
    public int Id { get; private set; }
    public int MaxCurrentInAmps { get; private set; }
    public Guid ChargeStationId { get; private set; }
    public ChargeStation ChargeStation { get; private set; } = null!;

    private Connector() { } // For EF Core

    public Connector(int id, int maxCurrentInAmps, Guid chargeStationId)
    {
        if (id < 1 || id > 5)
            throw new ArgumentException("Connector ID must be between 1 and 5.", nameof(id));

        if (maxCurrentInAmps <= 0)
            throw new ArgumentException("Max current must be greater than zero.", nameof(maxCurrentInAmps));

        Id = id;
        MaxCurrentInAmps = maxCurrentInAmps;
        ChargeStationId = chargeStationId;
    }

    public void UpdateMaxCurrent(int maxCurrentInAmps)
    {
        if (maxCurrentInAmps <= 0)
            throw new ArgumentException("Max current must be greater than zero.", nameof(maxCurrentInAmps));

        MaxCurrentInAmps = maxCurrentInAmps;
    }
}


