using FluentValidation;
using SmartChargingManagement.Application.Features.ChargeStations.Commands;

namespace SmartChargingManagement.Application.Features.ChargeStations.Validators;

public class UpdateChargeStationCommandValidator : AbstractValidator<UpdateChargeStationCommand>
{
    public UpdateChargeStationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}


