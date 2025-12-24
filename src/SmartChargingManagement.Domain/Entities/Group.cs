namespace SmartChargingManagement.Domain.Entities;

public class Group
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int CapacityInAmps { get; private set; }
    public ICollection<ChargeStation> ChargeStations { get; private set; } = new List<ChargeStation>();

    private Group() { } // For EF Core

    public Group(Guid id, string name, int capacityInAmps)
    {
        if (capacityInAmps <= 0)
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacityInAmps));

        Id = id;
        Name = name;
        CapacityInAmps = capacityInAmps;
        ChargeStations = new List<ChargeStation>();
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

        Name = name;
    }

    public void UpdateCapacity(int capacityInAmps)
    {
        if (capacityInAmps <= 0)
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacityInAmps));

        // Validate that new capacity is at least the sum of all connector max currents
        var totalMaxCurrent = ChargeStations
            .SelectMany(cs => cs.Connectors)
            .Sum(c => c.MaxCurrentInAmps);

        if (capacityInAmps < totalMaxCurrent)
            throw new InvalidOperationException(
                $"Group capacity ({capacityInAmps} Amps) must be greater than or equal to the sum of all connector max currents ({totalMaxCurrent} Amps).");

        CapacityInAmps = capacityInAmps;
    }

    public void AddChargeStation(ChargeStation chargeStation)
    {
        if (chargeStation == null)
            throw new ArgumentNullException(nameof(chargeStation));

        ChargeStations.Add(chargeStation);
    }

    public void RemoveChargeStation(ChargeStation chargeStation)
    {
        if (chargeStation == null)
            throw new ArgumentNullException(nameof(chargeStation));

        ChargeStations.Remove(chargeStation);
    }

    public int GetTotalConnectorMaxCurrent()
    {
        return ChargeStations
            .SelectMany(cs => cs.Connectors)
            .Sum(c => c.MaxCurrentInAmps);
    }
}

