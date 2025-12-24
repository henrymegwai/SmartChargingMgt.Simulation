using SmartChargingManagement.Domain.Entities;

namespace SmartChargingManagement.Application.Common.Interfaces;

public interface IChargeStationRepository : IRepository<ChargeStation>
{
    Task<ChargeStation?> GetByIdWithConnectorsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChargeStation>> GetAllWithConnectorsAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsInGroupAsync(Guid chargeStationId, Guid groupId, CancellationToken cancellationToken = default);
}

