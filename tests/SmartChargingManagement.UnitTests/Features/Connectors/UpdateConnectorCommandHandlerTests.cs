using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Features.Connectors.Commands;
using SmartChargingManagement.Application.Features.Connectors.Handlers;
using SmartChargingManagement.Domain.Entities;

namespace SmartChargingManagement.UnitTests.Features.Connectors;

public class UpdateConnectorCommandHandlerTests
{
    private void SetChargeStation(Connector connector, ChargeStation chargeStation)
    {
        var property = typeof(Connector).GetProperty("ChargeStation", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        property?.SetValue(connector, chargeStation);
    }


    [Fact]
    public async Task Handle_ShouldUpdateConnectorMaxCurrent_WhenValidCommand()
    {
        // Arrange
        var connectorRepository = Substitute.For<IConnectorRepository>();
        var groupRepository = Substitute.For<IGroupRepository>();
        var logger = Substitute.For<ILogger<UpdateConnectorCommandHandler>>();
        var handler = new UpdateConnectorCommandHandler(connectorRepository, groupRepository, logger);

        var groupId = Guid.NewGuid();
        var chargeStationId = Guid.NewGuid();
        var group = new Group(groupId, "Test Group", 200);
        var chargeStation = new ChargeStation(chargeStationId, "Test Charge Station", groupId);
        var connector = new Connector(1, 50, chargeStationId);
        chargeStation.AddConnector(connector);
        SetChargeStation(connector, chargeStation); // Set navigation property as EF Core would
        group.AddChargeStation(chargeStation);

        connectorRepository.GetByIdAndChargeStationIdAsync(1, chargeStationId, Arg.Any<CancellationToken>())
            .Returns(connector);

        groupRepository.GetByIdWithChargeStationsAsync(groupId, Arg.Any<CancellationToken>())
            .Returns(group);

        connectorRepository.UpdateAsync(Arg.Any<Connector>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var command = new UpdateConnectorCommand(1, chargeStationId, 75);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(1);
        result.Data.MaxCurrentInAmps.Should().Be(75);
        result.Data.ChargeStationId.Should().Be(chargeStationId);
        result.Message.Should().Be("Connector updated successfully");
        connector.MaxCurrentInAmps.Should().Be(75);
        await connectorRepository.Received(1).UpdateAsync(Arg.Any<Connector>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailedResponse_WhenConnectorNotFound()
    {
        // Arrange
        var connectorRepository = Substitute.For<IConnectorRepository>();
        var groupRepository = Substitute.For<IGroupRepository>();
        var logger = Substitute.For<ILogger<UpdateConnectorCommandHandler>>();
        var handler = new UpdateConnectorCommandHandler(connectorRepository, groupRepository, logger);
        var chargeStationId = Guid.NewGuid();

        connectorRepository.GetByIdAndChargeStationIdAsync(1, chargeStationId, Arg.Any<CancellationToken>())
            .Returns((Connector?)null);

        var command = new UpdateConnectorCommand(1, chargeStationId, 75);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Contain("Connector with ID");
        result.Message.Should().Contain("was not found");
        await connectorRepository.DidNotReceive().UpdateAsync(Arg.Any<Connector>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailedResponse_WhenGroupCapacityExceeded()
    {
        // Arrange
        var connectorRepository = Substitute.For<IConnectorRepository>();
        var groupRepository = Substitute.For<IGroupRepository>();
        var logger = Substitute.For<ILogger<UpdateConnectorCommandHandler>>();
        var handler = new UpdateConnectorCommandHandler(connectorRepository, groupRepository, logger);

        var groupId = Guid.NewGuid();
        var chargeStationId = Guid.NewGuid();
        var group = new Group(groupId, "Test Group", 100);
        var chargeStation = new ChargeStation(chargeStationId, "Test Charge Station", groupId);
        var connector = new Connector(1, 50, chargeStationId);
        chargeStation.AddConnector(connector);
        SetChargeStation(connector, chargeStation); // Set navigation property as EF Core would
        group.AddChargeStation(chargeStation);

        connectorRepository.GetByIdAndChargeStationIdAsync(1, chargeStationId, Arg.Any<CancellationToken>())
            .Returns(connector);

        groupRepository.GetByIdWithChargeStationsAsync(groupId, Arg.Any<CancellationToken>())
            .Returns(group);

        // Update connector from 50 to 150, which exceeds group capacity of 100
        var exceedingCommand = new UpdateConnectorCommand(1, chargeStationId, 150);

        // Act
        var result = await handler.Handle(exceedingCommand, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Message.Should().Contain("Group capacity");
        result.Message.Should().Contain("would be less than");
        await connectorRepository.DidNotReceive().UpdateAsync(Arg.Any<Connector>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailedResponse_WhenChargeStationIsNull()
    {
        // Arrange
        var connectorRepository = Substitute.For<IConnectorRepository>();
        var groupRepository = Substitute.For<IGroupRepository>();
        var logger = Substitute.For<ILogger<UpdateConnectorCommandHandler>>();
        var handler = new UpdateConnectorCommandHandler(connectorRepository, groupRepository, logger);

        var chargeStationId = Guid.NewGuid();
        var connector = new Connector(1, 50, chargeStationId);
        // Don't set ChargeStation navigation property (as if EF Core didn't load it)

        connectorRepository.GetByIdAndChargeStationIdAsync(1, chargeStationId, Arg.Any<CancellationToken>())
            .Returns(connector);

        var command = new UpdateConnectorCommand(1, chargeStationId, 75);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Contain("Charge station information is missing");
        await connectorRepository.DidNotReceive().UpdateAsync(Arg.Any<Connector>(), Arg.Any<CancellationToken>());
    }
}

