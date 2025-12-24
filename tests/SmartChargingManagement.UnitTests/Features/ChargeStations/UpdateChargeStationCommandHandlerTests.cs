using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.ChargeStations.Commands;
using SmartChargingManagement.Application.Features.ChargeStations.Handlers;
using SmartChargingManagement.Domain.Entities;
using Xunit;

namespace SmartChargingManagement.UnitTests.Features.ChargeStations;

public class UpdateChargeStationCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldUpdateChargeStationName_WhenValidCommand()
    {
        // Arrange
        var repository = Substitute.For<IChargeStationRepository>();
        var logger = Substitute.For<ILogger<UpdateChargeStationCommandHandler>>();
        var handler = new UpdateChargeStationCommandHandler(repository, logger);
        var chargeStationId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var existingChargeStation = new ChargeStation(chargeStationId, "Old Name", groupId);

        repository.GetByIdWithConnectorsAsync(chargeStationId, Arg.Any<CancellationToken>())
            .Returns(existingChargeStation);
        repository.UpdateAsync(Arg.Any<ChargeStation>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var command = new UpdateChargeStationCommand(chargeStationId, "New Name");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("New Name");
        result.Data.Id.Should().Be(chargeStationId);
        result.Data.GroupId.Should().Be(groupId);
        result.Message.Should().Be("Charge station updated successfully");
        existingChargeStation.Name.Should().Be("New Name");
        await repository.Received(1).UpdateAsync(Arg.Is<ChargeStation>(cs => cs.Name == "New Name"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailedResponse_WhenNameIsNull()
    {
        // Arrange
        var repository = Substitute.For<IChargeStationRepository>();
        var logger = Substitute.For<ILogger<UpdateChargeStationCommandHandler>>();
        var handler = new UpdateChargeStationCommandHandler(repository, logger);
        var chargeStationId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var existingChargeStation = new ChargeStation(chargeStationId, "Original Name", groupId);

        repository.GetByIdWithConnectorsAsync(chargeStationId, Arg.Any<CancellationToken>())
            .Returns(existingChargeStation);

        var command = new UpdateChargeStationCommand(chargeStationId, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Contain("An error occurred while updating the charge station");
        await repository.DidNotReceive().UpdateAsync(Arg.Any<ChargeStation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailedResponse_WhenChargeStationNotFound()
    {
        // Arrange
        var repository = Substitute.For<IChargeStationRepository>();
        var logger = Substitute.For<ILogger<UpdateChargeStationCommandHandler>>();
        var handler = new UpdateChargeStationCommandHandler(repository, logger);
        var chargeStationId = Guid.NewGuid();

        repository.GetByIdWithConnectorsAsync(chargeStationId, Arg.Any<CancellationToken>())
            .Returns((ChargeStation?)null);

        var command = new UpdateChargeStationCommand(chargeStationId, "New Name");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Contain("Charge station with ID");
        result.Message.Should().Contain("was not found");
        await repository.DidNotReceive().UpdateAsync(Arg.Any<ChargeStation>(), Arg.Any<CancellationToken>());
    }
}

