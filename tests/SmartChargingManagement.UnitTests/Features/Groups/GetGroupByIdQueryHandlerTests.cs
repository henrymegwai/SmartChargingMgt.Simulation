using FluentAssertions;
using NSubstitute;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Groups.Handlers;
using SmartChargingManagement.Application.Features.Groups.Queries;
using SmartChargingManagement.Domain.Entities;
using Xunit;

namespace SmartChargingManagement.UnitTests.Features.Groups;

public class GetGroupByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnGroup_WhenGroupExists()
    {
        // Arrange
        var repository = Substitute.For<IGroupRepository>();
        var handler = new GetGroupByIdQueryHandler(repository);
        var groupId = Guid.NewGuid();
        var query = new GetGroupByIdQuery(groupId);

        var group = new Group(groupId, "Test Group", 100);
        repository.GetByIdAsync(groupId, Arg.Any<CancellationToken>())
            .Returns(group);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(groupId);
        result.Data.Name.Should().Be("Test Group");
        result.Data.CapacityInAmps.Should().Be(100);
        result.Message.Should().Be("Group retrieved successfully");
        await repository.Received(1).GetByIdAsync(groupId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNullData_WhenGroupNotFound()
    {
        // Arrange
        var repository = Substitute.For<IGroupRepository>();
        var handler = new GetGroupByIdQueryHandler(repository);
        var groupId = Guid.NewGuid();
        var query = new GetGroupByIdQuery(groupId);

        repository.GetByIdAsync(groupId, Arg.Any<CancellationToken>())
            .Returns((Group?)null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Group not found");
        await repository.Received(1).GetByIdAsync(groupId, Arg.Any<CancellationToken>());
    }
}

