using FluentValidation;

namespace UberSy.Application.Features.Identity.Commands.ConfirmPhone;

public sealed class ConfirmPhoneCommandValidator : AbstractValidator<ConfirmPhoneCommand>
{
    public ConfirmPhoneCommandValidator() =>
        RuleFor(x => x.Code).NotEmpty().Matches(@"^\d{6}$").WithMessage("Code Must Be 6 Digits.");
}
