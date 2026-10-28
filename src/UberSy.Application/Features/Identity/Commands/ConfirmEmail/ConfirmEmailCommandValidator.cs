using FluentValidation;

namespace UberSy.Application.Features.Identity.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Code).NotEmpty().Matches(@"^\d{6}$").WithMessage("Code Must Be 6 Digits.");
    }
}
