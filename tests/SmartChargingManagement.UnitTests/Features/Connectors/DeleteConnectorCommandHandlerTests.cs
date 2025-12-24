using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Features.Connectors.Commands;
using SmartChargingManagement.Application.Features.Connectors.Handlers;
using SmartChargingManagement.Domain.Entities;

namespace SmartChargingManagement.UnitTests.Features.Connectors;

public class DeleteConnectorCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldDeleteConnector_WhenConnectorExists()
    {
        // Arrange
        var connectorRepository = Substitute.For<IConnectorRepository>();
        var chargeStationRepository = Substitute.For<IChargeStationRepository>();
        var logger = Substitute.For<ILogger<DeleteConnectorCommandHandler>>();
        var handler = new DeleteConnectorCommandHandler(connectorRepository, chargeStationRepository, logger);

        var groupId = Guid.NewGuid();
        var chargeStationId = Guid.NewGuid();
        var chargeStation = new ChargeStation(chargeStationId, "Test Charge Station", groupId);
        var connector1 = new Connector(1, 50, chargeStationId);
        var connector2 = new Connector(2, 50, chargeStationId);
        chargeStation.AddConnector(connector1);
        chargeStation.AddConnector(connector2);

        connectorRepository.GetByIdAndChargeStationIdAsync(1, chargeStationId, Arg.Any<CancellationToken>())
            .Returns(connector1);

        chargeStationRepository.GetByIdWithConnectorsAsync(chargeStationId, Arg.Any<CancellationToken>())
            .Returns(chargeStation);

        connectorRepository.DeleteAsync(Arg.Any<Connector>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var command = new DeleteConnectorCommand(1, chargeStationId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().Be(string.Empty);
        result.Message.Should().Be("Connector deleted successfully");
        await connectorRepository.Received(1).GetByIdAndChargeStationIdAsync(1, chargeStationId, Arg.Any<CancellationToken>());
        await chargeStationRepository.Received(1).GetByIdWithConnectorsAsync(chargeStationId, Arg.Any<CancellationToken>());
        await connectorRepository.Received(1).DeleteAsync(Arg.Is<Connector>(c => c.Id == 1), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailedResponse_WhenConnectorNotFound()
    {
        // Arrange
        var connectorRepository = Substitute.For<IConnectorRepository>();
        var chargeStationRepository = Substitute.For<IChargeStationRepository>();
        var logger = Substitute.For<ILogger<DeleteConnectorCommandHandler>>();
        var handler = new DeleteConnectorCommandHandler(connectorRepository, chargeStationRepository, logger);
        var chargeStationId = Guid.NewGuid();

        connectorRepository.GetByIdAndChargeStationIdAsync(1, chargeStationId, Arg.Any<CancellationToken>())
            .Returns((Connector?)null);

        var command = new DeleteConnectorCommand(1, chargeStationId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().Be(string.Empty);
        result.Message.Should().Contain("Connector with ID");
        result.Message.Should().Contain("was not found");
        await connectorRepository.DidNotReceive().DeleteAsync(Arg.Any<Connector>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailedResponse_WhenChargeStationNotFound()
    {
        // Arrange
        var connectorRepository = Substitute.For<IConnectorRepository>();
        var chargeStationRepository = Substitute.For<IChargeStationRepository>();
        var logger = Substitute.For<ILogger<DeleteConnectorCommandHandler>>();
        var handler = new DeleteConnectorCommandHandler(connectorRepository, chargeStationRepository, logger);
        var chargeStationId = Guid.NewGuid();
        var connector = new Connector(1, 50, chargeStationId);

        connectorRepository.GetByIdAndChargeStationIdAsync(1, chargeStationId, Arg.Any<CancellationToken>())
            .Returns(connector);

        chargeStationRepository.GetByIdWithConnectorsAsync(chargeStationId, Arg.Any<CancellationToken>())
            .Returns((ChargeStation?)null);

        var command = new DeleteConnectorCommand(1, chargeStationId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().Be(string.Empty);
        result.Message.Should().Contain("Charge station with ID");
        result.Message.Should().Contain("was not found");
        await connectorRepository.DidNotReceive().DeleteAsync(Arg.Any<Connector>(), Arg.Any<CancellationToken>());
    }
}

