using Microsoft.EntityFrameworkCore;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Domain.Entities;
using SmartChargingManagement.Infrastructure.Data;

namespace SmartChargingManagement.Infrastructure.Repositories;

public class ConnectorRepository : Repository<Connector>, IConnectorRepository
{
    public ConnectorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Connector?> GetByIdAndChargeStationIdAsync(int id, Guid chargeStationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.ChargeStation)
                .ThenInclude(cs => cs!.Group)
            .FirstOrDefaultAsync(c => c.Id == id && c.ChargeStationId == chargeStationId, cancellationToken);
    }

    public async Task<IEnumerable<Connector>> GetByChargeStationIdAsync(Guid chargeStationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.ChargeStationId == chargeStationId)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Connector?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        // For Connector, we need both Id and ChargeStationId, so this method should not be used directly
        throw new InvalidOperationException("Use GetByIdAndChargeStationIdAsync instead for Connector.");
    }
}


