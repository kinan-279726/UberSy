using FluentValidation;
using UberSy.Application.Common.Helpers;

namespace UberSy.Application.Features.Identity.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty()
                .Must(p => PhoneNumberNormalizer.ToE164(p) is not null).WithMessage("Phone Number Is Not Valid.");
    }
}
