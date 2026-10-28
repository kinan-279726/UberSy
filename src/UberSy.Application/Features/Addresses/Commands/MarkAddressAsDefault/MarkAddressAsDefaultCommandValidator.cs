using FluentValidation;
public sealed class MarkAddressAsDefaultCommandValidator : AbstractValidator<MarkAddressAsDefaultCommand>
{
    public MarkAddressAsDefaultCommandValidator()
    {
        RuleFor(x => x.AddressId).NotEmpty();
    }
}
