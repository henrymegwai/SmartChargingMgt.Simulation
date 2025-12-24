using Microsoft.EntityFrameworkCore;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Domain.Entities;
using SmartChargingManagement.Infrastructure.Data;

namespace SmartChargingManagement.Infrastructure.Repositories;

public class ConnectorRepository(ApplicationDbContext context) : Repository<Connector>(context), IConnectorRepository
{
    public async Task<Connector?> GetByIdAndChargeStationIdAsync(int id, Guid chargeStationId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.ChargeStation)
                .ThenInclude(cs => cs!.Group)
            .FirstOrDefaultAsync(c => c.Id == id && c.ChargeStationId == chargeStationId, cancellationToken);
    }

    public async Task<Connector?> GetByChargeStationIdAsync(Guid chargeStationId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(c => c.ChargeStationId == chargeStationId, cancellationToken);
    }
}
