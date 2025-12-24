using SmartChargingManagement.Domain.Entities;

namespace SmartChargingManagement.Application.Common.Interfaces;

public interface IConnectorRepository : IRepository<Connector>
{
    Task<Connector?> GetByIdAndChargeStationIdAsync(int id, Guid chargeStationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Connector>> GetByChargeStationIdAsync(Guid chargeStationId, CancellationToken cancellationToken = default);
}


