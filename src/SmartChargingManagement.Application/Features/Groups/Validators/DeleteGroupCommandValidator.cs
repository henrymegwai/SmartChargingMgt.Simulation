using FluentValidation;
using SmartChargingManagement.Application.Features.Groups.Commands;

namespace SmartChargingManagement.Application.Features.Groups.Validators;

public class DeleteGroupCommandValidator : AbstractValidator<DeleteGroupCommand>
{
    public DeleteGroupCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}


