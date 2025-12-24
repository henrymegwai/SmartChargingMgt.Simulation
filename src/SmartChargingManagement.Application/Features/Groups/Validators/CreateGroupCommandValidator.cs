using FluentValidation;
using SmartChargingManagement.Application.Features.Groups.Commands;

namespace SmartChargingManagement.Application.Features.Groups.Validators;

public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.CapacityInAmps)
            .GreaterThan(0).WithMessage("Capacity must be greater than zero.");
    }
}


