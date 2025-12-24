using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Infrastructure.Data;
using SmartChargingManagement.Infrastructure.Repositories;

namespace SmartChargingManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=SmartCharging.db";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IChargeStationRepository, ChargeStationRepository>();
        services.AddScoped<IConnectorRepository, ConnectorRepository>();

        return services;
    }
}

