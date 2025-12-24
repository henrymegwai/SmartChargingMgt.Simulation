namespace SmartChargingManagement.Domain.Entities;

public class ChargeStation
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Guid GroupId { get; private set; }
    public Group Group { get; private set; } = null!;
    public ICollection<Connector> Connectors { get; private set; }

    public ChargeStation(Guid id, string name, Guid groupId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

        Id = id;
        Name = name;
        GroupId = groupId;
        Connectors = new List<Connector>();
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

        Name = name;
    }

    public void AddConnector(Connector connector)
    {
        ArgumentNullException.ThrowIfNull(connector);

        if (Connectors.Count >= 5)
            throw new InvalidOperationException("A charge station cannot have more than 5 connectors.");

        if (Connectors.Any(c => c.Id == connector.Id))
            throw new InvalidOperationException($"Connector with ID {connector.Id} already exists in this charge station.");

        Connectors.Add(connector);
    }

    public void RemoveConnector(Connector connector)
    {
        if (connector == null)
            throw new ArgumentNullException(nameof(connector));

        if (Connectors.Count == 1)
            throw new InvalidOperationException("A charge station must have at least one connector.");

        Connectors.Remove(connector);
    }
}

