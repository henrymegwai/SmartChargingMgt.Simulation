using FluentAssertions;
using NSubstitute;
using SmartChargingManagement.Application.Common.Interfaces;
using SmartChargingManagement.Application.Common.Models;
using SmartChargingManagement.Application.Features.Groups.Handlers;
using SmartChargingManagement.Application.Features.Groups.Queries;
using SmartChargingManagement.Domain.Entities;
using Xunit;

namespace SmartChargingManagement.UnitTests.Features.Groups;

public class GetAllGroupsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAllGroups_WhenGroupsExist()
    {
        // Arrange
        var repository = Substitute.For<IGroupRepository>();
        var handler = new GetAllGroupsQueryHandler(repository);
        var query = new GetAllGroupsQuery();

        var group1 = new Group(Guid.NewGuid(), "Group 1", 100);
        var group2 = new Group(Guid.NewGuid(), "Group 2", 200);
        var groups = new List<Group> { group1, group2 };

        repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(groups);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data[0].Name.Should().Be("Group 1");
        result.Data[0].CapacityInAmps.Should().Be(100);
        result.Data[1].Name.Should().Be("Group 2");
        result.Data[1].CapacityInAmps.Should().Be(200);
        result.Message.Should().Be("Groups retrieved successfully");
        await repository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoGroupsExist()
    {
        // Arrange
        var repository = Substitute.For<IGroupRepository>();
        var handler = new GetAllGroupsQueryHandler(repository);
        var query = new GetAllGroupsQuery();

        repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Group>());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.Message.Should().Be("Groups retrieved successfully");
        await repository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }
}

