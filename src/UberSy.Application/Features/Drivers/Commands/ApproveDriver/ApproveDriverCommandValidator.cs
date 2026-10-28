using FluentValidation;

namespace UberSy.Application.Features.Drivers.Commands.ApproveDriver;

public sealed class ApproveDriverCommandValidator : AbstractValidator<ApproveDriverCommand>
{
    public ApproveDriverCommandValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty();
        RuleFor(x => x.ServiceTier).IsInEnum();
    }
}