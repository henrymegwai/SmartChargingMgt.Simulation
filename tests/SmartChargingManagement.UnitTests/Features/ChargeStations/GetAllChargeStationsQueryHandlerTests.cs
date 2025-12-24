using FluentAssertions;
using NSubstitute;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.ChargeStations.Handlers;
using SmartChargingManagement.Application.Features.ChargeStations.Queries;
using SmartChargingManagement.Domain.Entities;
using Xunit;

namespace SmartChargingManagement.UnitTests.Features.ChargeStations;

public class GetAllChargeStationsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAllChargeStations_WhenChargeStationsExist()
    {
        // Arrange
        var repository = Substitute.For<IChargeStationRepository>();
        var handler = new GetAllChargeStationsQueryHandler(repository);
        var query = new GetAllChargeStationsQuery();

        var groupId = Guid.NewGuid();
        var chargeStation1 = new ChargeStation(Guid.NewGuid(), "Charge Station 1", groupId);
        var chargeStation2 = new ChargeStation(Guid.NewGuid(), "Charge Station 2", groupId);
        var chargeStations = new List<ChargeStation> { chargeStation1, chargeStation2 };

        repository.GetAllWithConnectorsAsync(Arg.Any<CancellationToken>())
            .Returns(chargeStations);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data[0].Name.Should().Be("Charge Station 1");
        result.Data[1].Name.Should().Be("Charge Station 2");
        result.Message.Should().Be("Charge stations retrieved successfully");
        await repository.Received(1).GetAllWithConnectorsAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoChargeStationsExist()
    {
        // Arrange
        var repository = Substitute.For<IChargeStationRepository>();
        var handler = new GetAllChargeStationsQueryHandler(repository);
        var query = new GetAllChargeStationsQuery();

        repository.GetAllWithConnectorsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<ChargeStation>());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.Message.Should().Be("Charge stations retrieved successfully");
        await repository.Received(1).GetAllWithConnectorsAsync(Arg.Any<CancellationToken>());
    }
}

