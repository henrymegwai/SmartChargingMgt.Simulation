using Microsoft.EntityFrameworkCore;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Domain.Entities;
using SmartChargingManagement.Infrastructure.Data;

namespace SmartChargingManagement.Infrastructure.Repositories;

public class GroupRepository(ApplicationDbContext context) : Repository<Group>(context), IGroupRepository
{
    public async Task<Group?> GetByIdWithChargeStationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(g => g.ChargeStations)
                .ThenInclude(cs => cs.Connectors)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }
}


