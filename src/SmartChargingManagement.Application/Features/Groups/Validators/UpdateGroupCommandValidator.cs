using FluentValidation;
using SmartChargingManagement.Application.Features.Groups.Commands;

namespace SmartChargingManagement.Application.Features.Groups.Validators;

public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        When(x => x.Name != null, () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
        });

        When(x => x.CapacityInAmps.HasValue, () =>
        {
            RuleFor(x => x.CapacityInAmps!.Value)
                .GreaterThan(0).WithMessage("Capacity must be greater than zero.");
        });

        RuleFor(x => x)
            .Must(x => x.Name != null || x.CapacityInAmps.HasValue)
            .WithMessage("At least one field (Name or CapacityInAmps) must be provided.");
    }
}


