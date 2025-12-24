using FluentAssertions;
using NSubstitute;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Features.Groups.Commands;
using SmartChargingManagement.Application.Features.Groups.Handlers;
using SmartChargingManagement.Domain.Entities;

namespace SmartChargingManagement.UnitTests.Features.Groups;

public class CreateGroupCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateGroup_WhenValidCommand()
    {
        // Arrange
        var repository = Substitute.For<IGroupRepository>();
        var handler = new CreateGroupCommandHandler(repository);
        var command = new CreateGroupCommand("Test Group", 100);

        Group? savedGroup = null;
        repository.AddAsync(Arg.Do<Group>(g => savedGroup = g), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<Group>()));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be("Test Group");
        result.Data.CapacityInAmps.Should().Be(100);
        result.Message.Should().Be("Group created successfully");
        savedGroup.Should().NotBeNull();
        savedGroup!.Name.Should().Be("Test Group");
        savedGroup.CapacityInAmps.Should().Be(100);
        await repository.Received(1).AddAsync(Arg.Any<Group>(), Arg.Any<CancellationToken>());
    }
}


