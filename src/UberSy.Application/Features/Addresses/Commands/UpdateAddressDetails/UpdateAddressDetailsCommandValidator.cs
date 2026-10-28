using FluentValidation;

namespace UberSy.Application.Features.Addresses.Commands.UpdateAddressDetails;

public sealed class UpdateAddressDetailsCommandValidator : AbstractValidator<UpdateAddressDetailsCommand>
{
    public UpdateAddressDetailsCommandValidator()
    {
        RuleFor(x => x.AddressId).NotEmpty();
        RuleFor(x => x.Street).NotEmpty().MaximumLength(100);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
    }
}
