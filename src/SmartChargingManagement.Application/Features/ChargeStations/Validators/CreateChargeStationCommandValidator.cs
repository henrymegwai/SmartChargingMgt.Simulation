using FluentValidation;
using SmartChargingManagement.Application.Features.ChargeStations.Commands;

namespace SmartChargingManagement.Application.Features.ChargeStations.Validators;

public class CreateChargeStationCommandValidator : AbstractValidator<CreateChargeStationCommand>
{
    public CreateChargeStationCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required.");
    }
}


