using FluentAssertions;
using NSubstitute;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Connectors.Commands;
using SmartChargingManagement.Application.Features.Connectors.Handlers;
using SmartChargingManagement.Domain.Entities;
using Xunit;

namespace SmartChargingManagement.UnitTests.Features.Connectors;

public class CreateConnectorCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateConnector_WhenValidCommand()
    {
        // Arrange
        var connectorRepository = Substitute.For<IConnectorRepository>();
        var chargeStationRepository = Substitute.For<IChargeStationRepository>();
        var groupRepository = Substitute.For<IGroupRepository>();
        
        var groupId = Guid.NewGuid();
        var chargeStationId = Guid.NewGuid();
        var group = new Group(groupId, "Test Group", 100);
        var chargeStation = new ChargeStation(chargeStationId, "Test Charge Station", groupId);
        
        chargeStationRepository.GetByIdWithConnectorsAsync(chargeStationId, Arg.Any<CancellationToken>())
            .Returns(chargeStation);
        
        groupRepository.GetByIdWithChargeStationsAsync(groupId, Arg.Any<CancellationToken>())
            .Returns(group);
        
        connectorRepository.GetByIdAndChargeStationIdAsync(1, chargeStationId, Arg.Any<CancellationToken>())
            .Returns((Connector?)null);

        Connector? savedConnector = null;
        connectorRepository.AddAsync(Arg.Do<Connector>(c => savedConnector = c), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<Connector>()));

        var handler = new CreateConnectorCommandHandler(connectorRepository, chargeStationRepository, groupRepository);
        var command = new CreateConnectorCommand(1, 50, chargeStationId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(1);
        result.Data.MaxCurrentInAmps.Should().Be(50);
        result.Data.ChargeStationId.Should().Be(chargeStationId);
        result.Message.Should().Be("Connector created successfully");
        savedConnector.Should().NotBeNull();
        savedConnector!.Id.Should().Be(1);
        savedConnector.MaxCurrentInAmps.Should().Be(50);
        await connectorRepository.Received(1).AddAsync(Arg.Any<Connector>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailedResponse_WhenChargeStationNotFound()
    {
        // Arrange
        var connectorRepository = Substitute.For<IConnectorRepository>();
        var chargeStationRepository = Substitute.For<IChargeStationRepository>();
        var groupRepository = Substitute.For<IGroupRepository>();
        var chargeStationId = Guid.NewGuid();
        
        chargeStationRepository.GetByIdWithConnectorsAsync(chargeStationId, Arg.Any<CancellationToken>())
            .Returns((ChargeStation?)null);

        var handler = new CreateConnectorCommandHandler(connectorRepository, chargeStationRepository, groupRepository);
        var command = new CreateConnectorCommand(1, 50, chargeStationId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Contain("Charge station with ID");
        result.Message.Should().Contain("was not found");
        await connectorRepository.DidNotReceive().AddAsync(Arg.Any<Connector>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailedResponse_WhenConnectorAlreadyExists()
    {
        // Arrange
        var connectorRepository = Substitute.For<IConnectorRepository>();
        var chargeStationRepository = Substitute.For<IChargeStationRepository>();
        var groupRepository = Substitute.For<IGroupRepository>();
        
        var groupId = Guid.NewGuid();
        var chargeStationId = Guid.NewGuid();
        var group = new Group(groupId, "Test Group", 100);
        var chargeStation = new ChargeStation(chargeStationId, "Test Charge Station", groupId);
        var existingConnector = new Connector(1, 50, chargeStationId);
        
        chargeStationRepository.GetByIdWithConnectorsAsync(chargeStationId, Arg.Any<CancellationToken>())
            .Returns(chargeStation);
        
        connectorRepository.GetByIdAndChargeStationIdAsync(1, chargeStationId, Arg.Any<CancellationToken>())
            .Returns(existingConnector);

        var handler = new CreateConnectorCommandHandler(connectorRepository, chargeStationRepository, groupRepository);
        var command = new CreateConnectorCommand(1, 50, chargeStationId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Contain("already exists");
        await connectorRepository.DidNotReceive().AddAsync(Arg.Any<Connector>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailedResponse_WhenGroupCapacityExceeded()
    {
        // Arrange
        var connectorRepository = Substitute.For<IConnectorRepository>();
        var chargeStationRepository = Substitute.For<IChargeStationRepository>();
        var groupRepository = Substitute.For<IGroupRepository>();
        
        var groupId = Guid.NewGuid();
        var chargeStationId = Guid.NewGuid();
        var group = new Group(groupId, "Test Group", 100);
        var chargeStation = new ChargeStation(chargeStationId, "Test Charge Station", groupId);
        var existingConnector = new Connector(1, 60, chargeStationId);
        chargeStation.AddConnector(existingConnector);
        group.AddChargeStation(chargeStation);
        
        chargeStationRepository.GetByIdWithConnectorsAsync(chargeStationId, Arg.Any<CancellationToken>())
            .Returns(chargeStation);
        
        groupRepository.GetByIdWithChargeStationsAsync(groupId, Arg.Any<CancellationToken>())
            .Returns(group);
        
        connectorRepository.GetByIdAndChargeStationIdAsync(2, chargeStationId, Arg.Any<CancellationToken>())
            .Returns((Connector?)null);

        var handler = new CreateConnectorCommandHandler(connectorRepository, chargeStationRepository, groupRepository);
        // Try to add a connector with 50 Amps, but existing connector has 60, total would be 110, exceeding capacity of 100
        var command = new CreateConnectorCommand(2, 50, chargeStationId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Contain("Group capacity");
        result.Message.Should().Contain("would be less than");
        await connectorRepository.DidNotReceive().AddAsync(Arg.Any<Connector>(), Arg.Any<CancellationToken>());
    }
}

