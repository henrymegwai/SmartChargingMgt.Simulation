using SmartChargingManagement.Domain.Entities;

namespace SmartChargingManagement.Application.Common.Interfaces;

public interface IChargeStationRepository : IRepository<ChargeStation>
{
    Task<ChargeStation?> GetByIdWithConnectorsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChargeStation>> GetAllWithConnectorsAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsInGroupAsync(string name, Guid groupId, CancellationToken cancellationToken = default);
}

