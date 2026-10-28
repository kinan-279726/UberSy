using FluentValidation;

namespace UberSy.Application.Features.Drivers.Commands.TopUpWallet;

public sealed class TopUpWalletCommandValidator : AbstractValidator<TopUpWalletCommand> 
{
    public TopUpWalletCommandValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.DriverId).NotEmpty();
    }
}
