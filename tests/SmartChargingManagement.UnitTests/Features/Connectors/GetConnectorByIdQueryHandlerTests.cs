using FluentAssertions;
using NSubstitute;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Connectors.Handlers;
using SmartChargingManagement.Application.Features.Connectors.Queries;
using SmartChargingManagement.Domain.Entities;
using Xunit;

namespace SmartChargingManagement.UnitTests.Features.Connectors;

public class GetConnectorByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnConnector_WhenConnectorExists()
    {
        // Arrange
        var repository = Substitute.For<IConnectorRepository>();
        var handler = new GetConnectorByIdQueryHandler(repository);
        var chargeStationId = Guid.NewGuid();
        var query = new GetConnectorByIdQuery(1, chargeStationId);

        var connector = new Connector(1, 50, chargeStationId);
        repository.GetByIdAndChargeStationIdAsync(1, chargeStationId, Arg.Any<CancellationToken>())
            .Returns(connector);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(1);
        result.Data.MaxCurrentInAmps.Should().Be(50);
        result.Data.ChargeStationId.Should().Be(chargeStationId);
        result.Message.Should().Be("Connector retrieved successfully");
        await repository.Received(1).GetByIdAndChargeStationIdAsync(1, chargeStationId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNullData_WhenConnectorNotFound()
    {
        // Arrange
        var repository = Substitute.For<IConnectorRepository>();
        var handler = new GetConnectorByIdQueryHandler(repository);
        var chargeStationId = Guid.NewGuid();
        var query = new GetConnectorByIdQuery(1, chargeStationId);

        repository.GetByIdAndChargeStationIdAsync(1, chargeStationId, Arg.Any<CancellationToken>())
            .Returns((Connector?)null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Connector not found");
        await repository.Received(1).GetByIdAndChargeStationIdAsync(1, chargeStationId, Arg.Any<CancellationToken>());
    }
}

