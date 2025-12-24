using FluentAssertions;
using MediatR;
using NSubstitute;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Groups.Commands;
using SmartChargingManagement.Application.Features.Groups.Handlers;
using SmartChargingManagement.Domain.Entities;
using Xunit;

namespace SmartChargingManagement.UnitTests.Features.Groups;

public class DeleteGroupCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldDeleteGroup_WhenGroupExists()
    {
        // Arrange
        var repository = Substitute.For<IGroupRepository>();
        var groupId = Guid.NewGuid();
        var existingGroup = new Group(groupId, "Test Group", 100);
        
        repository.GetByIdAsync(groupId, Arg.Any<CancellationToken>())
            .Returns(existingGroup);

        var handler = new DeleteGroupCommandHandler(repository);
        var command = new DeleteGroupCommand(groupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().Be(string.Empty);
        result.Message.Should().Be("Group deleted successfully");
        await repository.Received(1).DeleteAsync(existingGroup, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailedResponse_WhenGroupNotFound()
    {
        // Arrange
        var repository = Substitute.For<IGroupRepository>();
        var groupId = Guid.NewGuid();
        
        repository.GetByIdAsync(groupId, Arg.Any<CancellationToken>())
            .Returns((Group?)null);

        var handler = new DeleteGroupCommandHandler(repository);
        var command = new DeleteGroupCommand(groupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeFalse();
        result.Data.Should().Be(string.Empty);
        result.Message.Should().Contain("Group with ID");
        result.Message.Should().Contain("was not found");
        await repository.DidNotReceive().DeleteAsync(Arg.Any<Group>(), Arg.Any<CancellationToken>());
    }
}


