using FluentValidation;
using SmartChargingManagement.Application.Features.ChargeStations.Commands;

namespace SmartChargingManagement.Application.Features.ChargeStations.Validators;

public class DeleteChargeStationCommandValidator : AbstractValidator<DeleteChargeStationCommand>
{
    public DeleteChargeStationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}


