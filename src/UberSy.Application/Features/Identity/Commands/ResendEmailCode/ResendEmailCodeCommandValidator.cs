using FluentValidation;

namespace UberSy.Application.Features.Identity.Commands.ResendEmailCode;

public sealed class ResendEmailCodeCommandValidator : AbstractValidator<ResendEmailCodeCommand>
{
    public ResendEmailCodeCommandValidator() => RuleFor(x => x.Email).NotEmpty().EmailAddress();
}
