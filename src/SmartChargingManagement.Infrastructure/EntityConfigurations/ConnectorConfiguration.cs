using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartChargingManagement.Domain.Entities;

namespace SmartChargingManagement.Infrastructure.EntityConfigurations;

public class ConnectorConfiguration: IEntityTypeConfiguration<Connector>
{
    public void Configure(EntityTypeBuilder<Connector> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.HasKey(e => new { e.Id, e.ChargeStationId });
        builder.Property(e => e.MaxCurrentInAmps).IsRequired();
        builder.Property(e => e.ChargeStationId)
            .IsRequired();
        builder.HasOne(e => e.ChargeStation)
            .WithMany(cs => cs.Connectors)
            .HasForeignKey(e => e.ChargeStationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}