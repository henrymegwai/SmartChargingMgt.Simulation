using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartChargingManagement.Domain.Entities;

namespace SmartChargingManagement.Infrastructure.EntityConfigurations;

public class ChargeStationConfiguration: IEntityTypeConfiguration<ChargeStation>
{
    public void Configure(EntityTypeBuilder<ChargeStation> builder)
    {
        // ChargeStation configuration
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.HasOne(e => e.Group)
            .WithMany(g => g.ChargeStations)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}