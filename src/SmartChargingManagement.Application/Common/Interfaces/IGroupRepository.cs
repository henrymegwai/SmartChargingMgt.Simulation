using SmartChargingManagement.Domain.Entities;

namespace SmartChargingManagement.Application.Common.Interfaces;

public interface IGroupRepository : IRepository<Group>
{
    Task<Group?> GetByIdWithChargeStationsAsync(Guid id, CancellationToken cancellationToken = default);
}


