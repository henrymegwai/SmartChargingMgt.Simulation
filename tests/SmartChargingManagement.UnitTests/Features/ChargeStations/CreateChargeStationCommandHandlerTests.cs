using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Features.ChargeStations.Commands;
using SmartChargingManagement.Application.Features.ChargeStations.Handlers;
using SmartChargingManagement.Domain.Entities;

namespace SmartChargingManagement.UnitTests.Features.ChargeStations;

public class CreateChargeStationCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateChargeStation_WhenValidCommand()
    {
        // Arrange
        var chargeStationRepository = Substitute.For<IChargeStationRepository>();
        var groupRepository = Substitute.For<IGroupRepository>();
        var groupId = Guid.NewGuid();
        var group = new Group(groupId, "Test Group", 100);
        
        groupRepository.GetByIdAsync(groupId, Arg.Any<CancellationToken>())
            .Returns(group);

        ChargeStation? savedChargeStation = null;
        chargeStationRepository.AddAsync(Arg.Do<ChargeStation>(cs => savedChargeStation = cs),
            Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<ChargeStation>()));

        var logger = Substitute.For<ILogger<CreateChargeStationCommandHandler>>();
        var handler = new CreateChargeStationCommandHandler(chargeStationRepository, groupRepository, logger);
        var command = new CreateChargeStationCommand("Test Charge Station", groupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be("Test Charge Station");
        result.Data.GroupId.Should().Be(groupId);
        result.Message.Should().Be("Charge station created successfully");
        savedChargeStation.Should().NotBeNull();
        savedChargeStation!.Name.Should().Be("Test Charge Station");
        savedChargeStation.GroupId.Should().Be(groupId);
        await chargeStationRepository.Received(1).AddAsync(Arg.Any<ChargeStation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailedResponse_WhenGroupNotFound()
    {
        // Arrange
        var chargeStationRepository = Substitute.For<IChargeStationRepository>();
        var groupRepository = Substitute.For<IGroupRepository>();
        var groupId = Guid.NewGuid();

        groupRepository.GetByIdAsync(groupId, Arg.Any<CancellationToken>())
            .Returns((Group?)null);

        var logger = Substitute.For<ILogger<CreateChargeStationCommandHandler>>();
        var handler = new CreateChargeStationCommandHandler(chargeStationRepository, groupRepository, logger);
        var command = new CreateChargeStationCommand("Test Charge Station", groupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Contain("Group with ID");
        result.Message.Should().Contain("was not found");
        await chargeStationRepository.DidNotReceive().AddAsync(Arg.Any<ChargeStation>(), Arg.Any<CancellationToken>());
    }
}


