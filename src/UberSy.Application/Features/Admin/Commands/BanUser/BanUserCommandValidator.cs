using FluentValidation;

namespace UberSy.Application.Features.Admin.Commands.BanUser;

public sealed class BanUserCommandValidator : AbstractValidator<BanUserCommand>
{
    public BanUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Until).GreaterThan(DateTime.UtcNow).When(x => x.Until.HasValue)
            .WithMessage("The ban end time must be in the future.");
    }
}