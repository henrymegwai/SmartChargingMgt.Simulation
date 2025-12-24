using FluentAssertions;
using SmartChargingManagement.Application.Features.Groups.Commands;
using SmartChargingManagement.Application.Features.Groups.Validators;
using Xunit;

namespace SmartChargingManagement.UnitTests.Validators;

public class CreateGroupCommandValidatorTests
{
    private readonly CreateGroupCommandValidator _validator;

    public CreateGroupCommandValidatorTests()
    {
        _validator = new CreateGroupCommandValidator();
    }

    [Fact]
    public void Validate_ShouldPass_WhenValidCommand()
    {
        // Arrange
        var command = new CreateGroupCommand("Test Group", 100);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateGroupCommand("", 100);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_ShouldFail_WhenCapacityIsZero()
    {
        // Arrange
        var command = new CreateGroupCommand("Test Group", 0);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CapacityInAmps");
    }

    [Fact]
    public void Validate_ShouldFail_WhenCapacityIsNegative()
    {
        // Arrange
        var command = new CreateGroupCommand("Test Group", -1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CapacityInAmps");
    }
}


