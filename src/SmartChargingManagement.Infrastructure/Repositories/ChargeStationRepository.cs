using Microsoft.EntityFrameworkCore;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Domain.Entities;
using SmartChargingManagement.Infrastructure.Data;

namespace SmartChargingManagement.Infrastructure.Repositories;

public class ChargeStationRepository(ApplicationDbContext context)
    : Repository<ChargeStation>(context), IChargeStationRepository
{
    public async Task<ChargeStation?> GetByIdWithConnectorsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(cs => cs.Connectors)
            .Include(cs => cs.Group)
            .FirstOrDefaultAsync(cs => cs.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ChargeStation>> GetAllWithConnectorsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Include(cs => cs.Connectors)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsInGroupAsync(string name, Guid groupId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .AnyAsync(cs => cs.Name.ToLower() == name.ToLower() && cs.GroupId == groupId, cancellationToken);
    }
}