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

public class DeleteChargeStationCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldDeleteChargeStation_WhenChargeStationExists()
    {
        // Arrange
        var repository = Substitute.For<IChargeStationRepository>();
        var logger = Substitute.For<ILogger<DeleteChargeStationCommandHandler>>();
        var handler = new DeleteChargeStationCommandHandler(repository, logger);
        var chargeStationId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var chargeStation = new ChargeStation(chargeStationId, "Test Charge Station", groupId);

        repository.GetByIdAsync(chargeStationId, Arg.Any<CancellationToken>())
            .Returns(chargeStation);
        repository.DeleteAsync(Arg.Any<ChargeStation>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var command = new DeleteChargeStationCommand(chargeStationId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().Be(string.Empty);
        result.Message.Should().Be("Charge station deleted successfully");
        await repository.Received(1).GetByIdAsync(chargeStationId, Arg.Any<CancellationToken>());
        await repository.Received(1).DeleteAsync(Arg.Is<ChargeStation>(cs => cs.Id == chargeStationId), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailedResponse_WhenChargeStationNotFound()
    {
        // Arrange
        var repository = Substitute.For<IChargeStationRepository>();
        var logger = Substitute.For<ILogger<DeleteChargeStationCommandHandler>>();
        var handler = new DeleteChargeStationCommandHandler(repository, logger);
        var chargeStationId = Guid.NewGuid();

        repository.GetByIdAsync(chargeStationId, Arg.Any<CancellationToken>())
            .Returns((ChargeStation?)null);

        var command = new DeleteChargeStationCommand(chargeStationId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().Be(string.Empty);
        result.Message.Should().Contain("Charge station with ID");
        result.Message.Should().Contain("was not found");
        await repository.DidNotReceive().DeleteAsync(Arg.Any<ChargeStation>(), Arg.Any<CancellationToken>());
    }
}

