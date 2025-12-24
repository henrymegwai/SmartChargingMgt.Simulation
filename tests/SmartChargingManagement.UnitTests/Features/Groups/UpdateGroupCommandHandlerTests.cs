using FluentAssertions;
using NSubstitute;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Groups.Commands;
using SmartChargingManagement.Application.Features.Groups.Handlers;
using SmartChargingManagement.Domain.Entities;
using Xunit;

namespace SmartChargingManagement.UnitTests.Features.Groups;

public class UpdateGroupCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldUpdateGroupName_WhenValidCommand()
    {
        // Arrange
        var repository = Substitute.For<IGroupRepository>();
        var groupId = Guid.NewGuid();
        var existingGroup = new Group(groupId, "Old Name", 100);
        
        repository.GetByIdWithChargeStationsAsync(groupId, Arg.Any<CancellationToken>())
            .Returns(existingGroup);

        var handler = new UpdateGroupCommandHandler(repository);
        var command = new UpdateGroupCommand(groupId, "New Name", null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be("New Name");
        result.Data.CapacityInAmps.Should().Be(100);
        result.Message.Should().Be("Group updated successfully");
        await repository.Received(1).UpdateAsync(Arg.Any<Group>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldUpdateGroupCapacity_WhenValidCommand()
    {
        // Arrange
        var repository = Substitute.For<IGroupRepository>();
        var groupId = Guid.NewGuid();
        var existingGroup = new Group(groupId, "Test Group", 100);
        
        repository.GetByIdWithChargeStationsAsync(groupId, Arg.Any<CancellationToken>())
            .Returns(existingGroup);

        var handler = new UpdateGroupCommandHandler(repository);
        var command = new UpdateGroupCommand(groupId, null, 200);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.CapacityInAmps.Should().Be(200);
        result.Message.Should().Be("Group updated successfully");
        await repository.Received(1).UpdateAsync(Arg.Any<Group>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailedResponse_WhenGroupNotFound()
    {
        // Arrange
        var repository = Substitute.For<IGroupRepository>();
        var groupId = Guid.NewGuid();
        
        repository.GetByIdWithChargeStationsAsync(groupId, Arg.Any<CancellationToken>())
            .Returns((Group?)null);

        var handler = new UpdateGroupCommandHandler(repository);
        var command = new UpdateGroupCommand(groupId, "New Name", null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Contain("Group with ID");
        result.Message.Should().Contain("was not found");
        await repository.DidNotReceive().UpdateAsync(Arg.Any<Group>(), Arg.Any<CancellationToken>());
    }
}


