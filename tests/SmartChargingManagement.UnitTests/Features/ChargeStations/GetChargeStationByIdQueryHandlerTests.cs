using FluentAssertions;
using NSubstitute;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.ChargeStations.Handlers;
using SmartChargingManagement.Application.Features.ChargeStations.Queries;
using SmartChargingManagement.Domain.Entities;
using Xunit;

namespace SmartChargingManagement.UnitTests.Features.ChargeStations;

public class GetChargeStationByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnChargeStation_WhenChargeStationExists()
    {
        // Arrange
        var repository = Substitute.For<IChargeStationRepository>();
        var handler = new GetChargeStationByIdQueryHandler(repository);
        var chargeStationId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var query = new GetChargeStationByIdQuery(chargeStationId);

        var chargeStation = new ChargeStation(chargeStationId, "Test Charge Station", groupId);
        repository.GetByIdWithConnectorsAsync(chargeStationId, Arg.Any<CancellationToken>())
            .Returns(chargeStation);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(chargeStationId);
        result.Data.Name.Should().Be("Test Charge Station");
        result.Data.GroupId.Should().Be(groupId);
        result.Message.Should().Be("Charge station retrieved successfully");
        await repository.Received(1).GetByIdWithConnectorsAsync(chargeStationId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNullData_WhenChargeStationNotFound()
    {
        // Arrange
        var repository = Substitute.For<IChargeStationRepository>();
        var handler = new GetChargeStationByIdQueryHandler(repository);
        var chargeStationId = Guid.NewGuid();
        var query = new GetChargeStationByIdQuery(chargeStationId);

        repository.GetByIdWithConnectorsAsync(chargeStationId, Arg.Any<CancellationToken>())
            .Returns((ChargeStation?)null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Charge station not found");
        await repository.Received(1).GetByIdWithConnectorsAsync(chargeStationId, Arg.Any<CancellationToken>());
    }
}

