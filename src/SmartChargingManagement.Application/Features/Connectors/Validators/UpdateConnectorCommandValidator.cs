using FluentValidation;
using SmartChargingManagement.Application.Features.Connectors.Commands;

namespace SmartChargingManagement.Application.Features.Connectors.Validators;

public class UpdateConnectorCommandValidator : AbstractValidator<UpdateConnectorCommand>
{
    public UpdateConnectorCommandValidator()
    {
        RuleFor(x => x.Id)
            .InclusiveBetween(1, 5).WithMessage("Connector ID must be between 1 and 5.");

        RuleFor(x => x.MaxCurrentInAmps)
            .GreaterThan(0).WithMessage("Max current must be greater than zero.");

        RuleFor(x => x.ChargeStationId)
            .NotEmpty().WithMessage("ChargeStationId is required.");
    }
}


