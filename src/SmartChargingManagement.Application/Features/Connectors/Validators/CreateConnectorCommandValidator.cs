using FluentValidation;
using SmartChargingManagement.Application.Features.Connectors.Commands;

namespace SmartChargingManagement.Application.Features.Connectors.Validators;

public class CreateConnectorCommandValidator : AbstractValidator<CreateConnectorCommand>
{
    public CreateConnectorCommandValidator()
    {
        RuleFor(x => x.MaxCurrentInAmps)
            .GreaterThan(0).WithMessage("Max current must be greater than zero.");

        RuleFor(x => x.ChargeStationId)
            .NotEmpty().WithMessage("ChargeStationId is required.");
    }
}


